using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fill_A_Pix_UI
{
    public class Scaler
    {
        public readonly float Scale;
        private readonly int _xOffset;
        private readonly int _yOffset;
        public readonly int ImageWidth;
        public readonly int ImageHeight;

        public Scaler( Control box, Image image ) : this( box.Size, image.Size) {}


        public Scaler( Size wSize, Size iSize )
        {
            ImageWidth = iSize.Width;
            ImageHeight = iSize.Height;

            var xScale = Convert.ToSingle( wSize.Width ) / Convert.ToSingle( iSize.Width );
            var yScale = Convert.ToSingle( wSize.Height ) / Convert.ToSingle( iSize.Height );
            Scale = Math.Min( xScale, yScale );

            // width smaller, so needs bigger scale -- x gets offset
            if( xScale > yScale )
            {
                _yOffset = 0;
                _xOffset = Convert.ToInt32( (xScale - yScale) * Convert.ToSingle( iSize.Width ) ) / 2;
            }
            // height smaller, so needs bigger scale -- y gets offset
            else
            {
                _xOffset = 0;
                _yOffset = Convert.ToInt32( (yScale - xScale) * Convert.ToSingle( iSize.Height ) ) / 2;
            }
        }


        public PointF ImageToScreen( int x, int y )
            => ImageToScreen( new Point( x, y ) );


        public PointF ImageToScreen( float x, float y )
            => ImageToScreen( new PointF( x, y ) );


        public PointF ImageToScreen( Point p )
        {
            var screenX = _xOffset + ( p.X * Scale);
            var screenY = _yOffset + ( p.Y * Scale);
            return new PointF( screenX, screenY );
        }


        public PointF ImageToScreen( PointF p )
        {
            var screenX = _xOffset + ( p.X * Scale);
            var screenY = _yOffset + ( p.Y * Scale);
            return new PointF( screenX, screenY );
        }
    }
}
