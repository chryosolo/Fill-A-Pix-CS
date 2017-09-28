using System.Drawing;
using System.Runtime.InteropServices;

namespace Fill_A_Pix_UI.Utility
{
    internal class Rect
    {
        [StructLayout( LayoutKind.Sequential )]
        public struct RECT
        {
            public RECT( RECT rect ) : this( rect.Left, rect.Top, rect.Right, rect.Bottom ) { }

            public RECT( int left, int top, int right, int bottom )
            {
                X = left;
                Y = top;
                Right = right;
                Bottom = bottom;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public int Left
            {
                get => X;
                set => X = value;
            }
            public int Top
            {
                get => Y;
                set => Y = value;
            }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Height
            {
                get => Bottom - Y;
                set => Bottom = value + Y;
            }
            public int Width
            {
                get => Right - X;
                set => Right = value + X;
            }
            public Point Location
            {
                get => new Point( Left, Top );
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }
            public Size Size
            {
                get => new Size( Width, Height );
                set
                {
                    Right = value.Width + X;
                    Bottom = value.Height + Y;
                }
            }

            public static implicit operator Rectangle( RECT rect )
            {
                return new Rectangle( rect.Left, rect.Top, rect.Width, rect.Height );
            }

            public static implicit operator RECT( Rectangle rect )
            {
                return new RECT( rect.Left, rect.Top, rect.Right, rect.Bottom );
            }

            public static bool operator ==( RECT rect1, RECT rect2 )
            {
                return rect1.Equals( rect2 );
            }

            public static bool operator !=( RECT rect1, RECT rect2 )
            {
                return !rect1.Equals( rect2 );
            }

            public override string ToString()
            {
                return $"{{Left: {X}; Top: {Y}; Right: {Right}; Bottom: {Bottom}}}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals( RECT rect )
            {
                return rect.Left == X && rect.Top == Y && rect.Right == Right
                    && rect.Bottom == Bottom;
            }

            public override bool Equals( object Object )
            {
                if( Object is RECT )
                {
                    return Equals( (RECT) Object );
                }

                if( Object is Rectangle )
                {
                    return Equals( new RECT( (Rectangle) Object ) );
                }

                return false;
            }
        }
    }
}
