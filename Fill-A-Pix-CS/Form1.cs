using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Akka.Actor;

namespace Fill_A_Pix_CS
{
    public enum UiClueState
    {
        Active,
        Used
    }


    public enum UiCellValue
    {
        Unknown,
        Empty,
        Filled
    }


    public partial class FrmMain : Form
    {
        private IActorRef _actor;
        private Scaler _scaler;
        private readonly LinkedList<UiCommand> _uiCommandsReceived;
        private LinkedListNode<UiCommand> _runTo;
        private int _cols;
        private int _rows;

        private readonly StringFormat _clueFormat =
            new StringFormat { Alignment = StringAlignment.Center };


        private readonly Pen _penLine =
            new Pen( Color.Red, 3 ) { DashPattern = new[] { 5.0F, 5.0F } };
        private readonly Pen _penClueHint = new Pen( Color.Blue, 2 );
        private readonly Pen _penClueBoundary = new Pen( Color.DimGray, 2 );
        private readonly Brush _brushFilled = new SolidBrush( Color.Black );
        private readonly Brush _brushEmpty = new SolidBrush( Color.White );
        private readonly Brush _brushUnknown = new SolidBrush( Color.FromArgb( 99, 99, 140 ) );

        public FrmMain()
        {
            InitializeComponent();

            FontAwesome.DefaultProperties.Size = 32;
            tsbOpen.Image = new FontAwesome.Properties( FontAwesome.Type.FolderOpenO )
                .AsImage()
                .StackWith( new FontAwesome.Properties( FontAwesome.Type.FolderOpen )
                    { Size=30, Location=new Point(1, 0), ForeColor = Color.Yellow } );
            tsbCapture.Image = new FontAwesome.Properties( FontAwesome.Type.WindowMaximize )
                .AsImage()
                .StackWith( new FontAwesome.Properties( FontAwesome.Type.Bullseye )
                    { Size = 24, Location = new Point( 3, 4 ), ForeColor = Color.Red } );
            tsbReset.Image = FontAwesome.Type.FastBackward.AsImage();
            tsbStepBack.Image = new FontAwesome.Properties( FontAwesome.Type.StepBackward )
                    { ForeColor = Color.Blue }
                .AsImage();
            tsbStep.Image = new FontAwesome.Properties( FontAwesome.Type.StepForward )
                    { ForeColor = Color.Blue }
                .AsImage();
            tsbSolve.Image= FontAwesome.Type.FastForward.AsImage();

            _uiCommandsReceived = new LinkedList<UiCommand>();
            //_penLine = new Pen( Color.Red, 3 ){ DashPattern = new[] { 5.0F, 5.0F } };
        }

        public void SetActor( IActorRef actor )
        {
            _actor = actor;
        }

        private void OpenMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog1.ShowDialog();
        }

        private void CaptureMenuItem_Click( object sender, EventArgs e )
        {

        }

        private void ResetMenuItem_Click( object sender, EventArgs e )
        {
            solveMenuItem.Checked = false;
            tsbSolve.Checked = false;
            _runTo = _uiCommandsReceived.First;
            UpdateEnabled();
            Refresh();
        }

        private void StepMenuItem_Click( object sender, EventArgs e )
        {
            if( _runTo?.Next == null )
            {
                return;
            }

            _runTo = _runTo.Next;
            UpdateEnabled();
            _runTo?.Value.Draw();
        }

        private void SolveMenuItem_Click( object sender, EventArgs e )
        {
            solveMenuItem.Checked = !solveMenuItem.Checked;
            tsbSolve.Checked = !tsbSolve.Checked;
            while( solveMenuItem.Checked && _runTo?.Next != null )
            {
                _runTo.Value.Draw();
                _runTo = _runTo.Next;
            }

            UpdateEnabled();
            _runTo?.Value.Draw();
        }

        private void StepBackMenuItem_Click( object sender, EventArgs e )
        {
            solveMenuItem.Checked = false;
            tsbSolve.Checked = false;
            if( _runTo?.Previous == null )
            {
                return;
            }

            _runTo = _runTo.Previous;
            UpdateEnabled();
            Refresh();
        }

        private void OpenFileDialog1_FileOk( object sender, CancelEventArgs e )
        {
            _actor.Tell( new UiImageOpen { Filename = openFileDialog1.FileName } );
        }

        private void Panel1_Paint( object sender, PaintEventArgs e )
        {
            if( tsbSolve.Checked )
                _runTo = _uiCommandsReceived.Last;

            var ptr = _runTo;
            while( ptr?.Value is UiOverlayCommand )
                ptr = ptr.Previous;
            do
            {
                ptr?.Value?.Draw();
                ptr = ptr?.Next;
            } while( ptr != null && ptr.Previous != _runTo );
        }

        public void LogSetWindow( Bitmap bitmap )
        {
            _uiCommandsReceived.Clear();
            LogShowImage( bitmap );
            _runTo = _uiCommandsReceived.First;

            Refresh();
        }

        public void UpdateEnabled()
        {
            var isFirst = _runTo?.Previous == null;
            var isLast = _runTo?.Next == null;

            resetMenuItem.Enabled = tsbReset.Enabled = !isFirst;
            stepBackMenuItem.Enabled = tsbStepBack.Enabled = !isFirst;
            stepMenuItem.Enabled = tsbStep.Enabled = !isLast;
        }

        public void AddUiShowImageCommand( Action action )
        {
            var autoDraw = solveMenuItem.Checked && _runTo != null && _runTo.Next == null;
            _uiCommandsReceived.AddLast( new UiShowImage( action ) );
            UpdateEnabled();
            if( autoDraw )
            {
                _runTo = _uiCommandsReceived.Last;
                _runTo.Value.Draw();
            }
        }

        public void AddUiOverlayCommand( Action action )
        {
            var autoDraw = solveMenuItem.Checked && _runTo != null && _runTo.Next == null;
            _uiCommandsReceived.AddLast( new UiOverlayCommand( action ) );
            UpdateEnabled();
            if( autoDraw )
            {
                _runTo = _uiCommandsReceived.Last;
                _runTo.Value.Draw();
            }
        }

        public void AddClear()
        {
            _uiCommandsReceived.AddLast( new UiShowImage( DoClear ) );
        }

        public void LogShowImage( Bitmap bmp ) => AddUiShowImageCommand( () => DoShowImage( bmp ) );
        public void LogDrawHLine( int y ) => AddUiOverlayCommand( () => DoDrawHLine( y ) );
        public void LogDrawVLine( int x ) => AddUiOverlayCommand( () => DoDrawVLine( x ) );
        public void LogSizing( int cols, int rows ) => AddUiOverlayCommand( () => DoRememberSizing( cols, rows ) );
        public void LogBlankOverlay( int col, int row )
            => AddUiOverlayCommand( () => DoShowClueOverlay( col, row, null ) );
        public void LogClueOverlay( int col, int row, int clue )
            => AddUiOverlayCommand( () => DoShowClueOverlay( col, row, clue ) );

        public void LogShowClue( int col, int row, int? number, UiClueState clue, UiCellValue cell )
            => AddUiOverlayCommand( () => DoShowClue( col, row, number, clue, cell ) );

        public void LogClear() => AddClear();


        public void DoClear()
        {
            _scaler = new Scaler( panel1.Size,
                new Size( _scaler.ImageWidth, _scaler.ImageHeight ) );
            using( var g = panel1.CreateGraphics() )
            {
                g.Clear( DefaultBackColor );
            }
        }


        public void DoShowImage( Bitmap bitmap )
        {
            _scaler = new Scaler( panel1, bitmap );
            var width = bitmap.Width;
            var height = bitmap.Height;
            var panelPoint = Point.Round( _scaler.ImageToScreen( 0, 0 ) );
            var panelEndPoint = Point.Round( _scaler.ImageToScreen( width, height ) );
            var panelSize = new Size( panelEndPoint.X-panelPoint.X, panelEndPoint.Y-panelPoint.Y );

            using( var g = panel1.CreateGraphics() )
            {
                g.Clear( DefaultBackColor );
                g.DrawImage( bitmap, new Rectangle( panelPoint, panelSize ),
                    new Rectangle( 0, 0, width, height ), GraphicsUnit.Pixel );
            }
        }


        public void DoDrawHLine( int y )
        {
            var width = panel1.ClientSize.Width;
            var yCoord = _scaler?.ImageToScreen( 0, y ).Y ?? y;
            var left = new PointF( 0, yCoord );
            var right = new PointF( width, yCoord );
            using( var g = panel1.CreateGraphics() )
            {
                g.DrawLine( _penLine, left, right );
            }
        }


        public void DoDrawVLine( int x )
        {
            var height = panel1.ClientSize.Height;
            var xCoord = _scaler?.ImageToScreen( x, 0 ).X ?? x;
            var top = new PointF( xCoord, 0 );
            var bottom = new PointF( xCoord, height );
            using( var g = panel1.CreateGraphics() )
            {
                g.DrawLine( _penLine, top, bottom );
            }
        }


        public void DoRememberSizing( int cols, int rows )
        {
            _cols = cols;
            _rows = rows;
            for( var xIdx = 1.0; xIdx < cols; xIdx++ )
            {
                DoDrawVLine( Convert.ToInt32( xIdx / Convert.ToDouble( cols )
                    * Convert.ToDouble( _scaler.ImageWidth ) ) );
            }
            for( var yIdx = 1.0; yIdx < rows; yIdx++ )
            {
                DoDrawHLine( Convert.ToInt32( yIdx / Convert.ToDouble( rows )
                    * Convert.ToDouble( _scaler.ImageHeight ) ) );
            }
        }

        public void DoShowClueOverlay( int col, int row, int? number )
        {
            var width = _scaler.ImageWidth;
            var height = _scaler.ImageHeight;
            var x0 = width * Convert.ToSingle( col ) / Convert.ToSingle( _cols ) + 2.0f;
            var x1 = width * Convert.ToSingle( col + 1 ) / Convert.ToSingle( _cols ) - 2.0f;
            var y0 = height * Convert.ToSingle( row ) / Convert.ToSingle( _rows ) + 2.0f;
            var y1 = height * Convert.ToSingle( row + 1 ) / Convert.ToSingle( _rows ) - 2.0f;
            var p0 = Point.Round( _scaler.ImageToScreen( x0, y0 ) );
            var p1 = Point.Round( _scaler.ImageToScreen( x1, y1 ) );

            using( var g = panel1.CreateGraphics() )
            {
                g.DrawRectangle( _penClueHint,
                    new Rectangle( p0.X, p0.Y, p1.X - p0.X, p1.Y - p0.Y ) );
                if( number.HasValue )
                    g.DrawString( number.Value.ToString(), DefaultFont,
                        new SolidBrush( Color.Blue ), p0.X + 4, p0.Y + 4 );
            }
        }


        public void DoShowClue( int col, int row, int? number, UiClueState clue, UiCellValue cell )
        {
            var width = _scaler.ImageWidth;
            var height = _scaler.ImageHeight;
            var x0 = width * Convert.ToSingle( col ) / Convert.ToSingle( _cols );
            var x1 = width * Convert.ToSingle( col + 1 ) / Convert.ToSingle( _cols );
            var y0 = height * Convert.ToSingle( row ) / Convert.ToSingle( _rows );
            var y1 = height * Convert.ToSingle( row + 1 ) / Convert.ToSingle( _rows );
            var p0 = Point.Round( _scaler.ImageToScreen( x0, y0 ) );
            var p1 = Point.Round( _scaler.ImageToScreen( x1, y1 ) );

            var rect = new Rectangle( p0.X, p0.Y, p1.X - p0.X, p1.Y - p0.Y );
            var fillBrush = cell == UiCellValue.Unknown
                ? _brushUnknown
                : cell == UiCellValue.Filled
                    ? _brushFilled
                    : _brushEmpty;
            var textBrush = clue == UiClueState.Used
                ? new SolidBrush( Color.DimGray )
                : cell == UiCellValue.Empty
                    ? new SolidBrush( Color.Black )
                    : new SolidBrush( Color.White );

            using( var g = panel1.CreateGraphics() )
            {
                g.FillRectangle( fillBrush, rect );
                g.DrawRectangle( _penClueBoundary, rect );
                if( number != null )
                    g.DrawString( number.ToString(),
                        new Font( DefaultFont.Name, _scaler.Scale * 20 ), textBrush, rect,
                        _clueFormat );
            }
        }


        private void FrmMain_Resize( object sender, EventArgs e )
        {
            panel1.Refresh();
        }

        private void panel1_Click( object sender, EventArgs e )
        {
            _actor.Tell( new UiGameStep() );
        }
    }


    public class UiImageOpen
    {
        public string Filename { get; set; }
    }


    public class UiGameStep { }


    public class UiGameSolve { }
}
