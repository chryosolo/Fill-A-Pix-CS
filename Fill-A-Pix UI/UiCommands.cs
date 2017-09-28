using System;
using System.Drawing;

namespace Fill_A_Pix_UI
{
    public class UiCommand
    {
        private readonly Action _drawAction;

        public UiCommand( Action action ) => _drawAction = action;

        public void Draw() => _drawAction.Invoke();
    }

    public class UiShowImage : UiCommand
    {
        public UiShowImage( Action action ) : base( action ) {}
    }

    public class UiOverlayCommand : UiCommand
    {
        public UiOverlayCommand( Action action ) : base( action ) { }
    }

    //public class UiDrawHLine : UiCommand
    //{
    //    public int YCoord;

    //    public override void DrawOn( FrmMain form )
    //        => form.DoDrawHLine( YCoord );
    //}

    //public class UiDrawVLine : UiCommand
    //{
    //    public int XCoord;
    //}

    //public class UiDrawRect : UiCommand
    //{
    //    public int Top;
    //    public int Bottom;
    //    public int Left;
    //    public int Right;
    //    public Pen Line;
    //    public Color Fill;
    //}

    //public class UiDrawText : UiCommand
    //{
    //    public int XCoord;
    //    public int YCoord;
    //    public int Size;
    //    public string Text;
    //    public Color Color;
    //}
}
