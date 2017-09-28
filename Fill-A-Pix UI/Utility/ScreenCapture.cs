using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fill_A_Pix_UI.Utility
{
    internal class ScreenCapture
    {
        [DllImport( "user32.dll" )]
        public static extern bool GetWindowRect( IntPtr hWnd, out Rect.RECT lpRect );
        [DllImport( "user32.dll" )]
        public static extern bool PrintWindow( IntPtr hWnd, IntPtr hdcBlt, int nFlags );

        public static Bitmap PrintWindow( IntPtr hwnd )
        {
            Rect.RECT rc;
            GetWindowRect( hwnd, out rc );

            var bmp = new Bitmap( rc.Width, rc.Height, PixelFormat.Format32bppArgb );
            var gfxBmp = Graphics.FromImage( bmp );
            var hdcBitmap = gfxBmp.GetHdc();

            PrintWindow( hwnd, hdcBitmap, 0 );

            gfxBmp.ReleaseHdc( hdcBitmap );
            gfxBmp.Dispose();

            return bmp;
        }
    }
}
