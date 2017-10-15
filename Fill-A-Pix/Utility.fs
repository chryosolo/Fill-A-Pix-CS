module Fill_A_Pix.Utility

open System.Drawing
open GameTypes


// return a clone of the given bitmap
let clone (bitmap:Bitmap) = new Bitmap( bitmap )


// clip the given bitmap using the given PixelRect
let clipBitmap rect (bitmap:Bitmap) =
    let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
    let boardRect = new Rectangle( l, t, r - l + 1, b - t + 1 )
    bitmap.Clone( boardRect, bitmap.PixelFormat )


// turn a Bitmap into a Pixels
let toPixels (bitmap:Bitmap) : Pixels =
    Array2D.init bitmap.Width bitmap.Height
        (fun x y -> bitmap.GetPixel(x, y) )

// return width and height from the given pixels
let getDimensions pixels = (Array2D.length1 pixels, Array2D.length2 pixels)
    

// turn a Pixels into a Bitmap
let toBitmap (pixels:Pixels) : Bitmap =
    let (width, height) = getDimensions pixels
    let bitmap = new Bitmap( width, height )
    for (xc,yc) in (Array.allPairs [|0 .. width-1|] [|0 .. height-1|]) do
        bitmap.SetPixel( xc, yc, pixels.[xc, yc] )
    bitmap


// clip the given Pixels using the given PixelRect
let clipPixels rect (pixels:Pixels) : Pixels =
    let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
    let (width, height) = (r-l + 1, b-t + 1)
    let clipped = Array2D.create width height Color.Black 
    Array2D.blit pixels l t clipped 0 0 width height
    clipped
