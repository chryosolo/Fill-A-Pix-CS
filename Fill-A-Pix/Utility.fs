module Fill_A_Pix.Utility

open System.Drawing
open GameTypes
open System.Runtime.InteropServices.ComTypes

// clip the given bitmap using the given PixelRect
let clipBitmap rect (bitmap:Bitmap) =
    let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
    let boardRect = new Rectangle( l, t, r - l + 1, b - t + 1 )
    bitmap.Clone( boardRect, bitmap.PixelFormat )

// return a clone the given bitmap
let clone (bitmap:Bitmap) = new Bitmap( bitmap )

// turn a Bitmap into a Pixels
let toPixels (bitmap:Bitmap) : Pixels =
    Array2D.init
        bitmap.Width
        bitmap.Height
        (fun x y -> bitmap.GetPixel(x, y) )
    
let toBitmap (pixels:Pixels) : Bitmap =
    let width = Array2D.length1 pixels
    let height = Array2D.length2 pixels
    let x = [|0 .. width-1|]
    let y = [|0 .. height-1|]
    let bitmap = new Bitmap( width, height )
    for (xc,yc) in (Array.allPairs x y) do
        bitmap.SetPixel( xc, yc, pixels.[xc, yc] )
    bitmap

let clipPixels rect (pixels:Pixels) : Pixels =
    let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
    let width = r-l + 1
    let height = b-t + 1
    let clipped = Array2D.create width height Color.Black 
    Array2D.blit pixels l t clipped 0 0 width height
    clipped