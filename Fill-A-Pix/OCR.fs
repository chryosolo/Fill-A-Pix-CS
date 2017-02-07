module OCR

open System.Drawing

type CharTemplate = 
    | CharTemplate of Bitmap

type Rgb = byte * byte * byte

let colorToRgb (color:Color) =
    ( color.R, color.G, color.B )

let calcDiff (rgb1:Rgb) (rgb2:Rgb) =
    let (r1, g1, b1) = rgb1
    let (r2, g2, b2) = rgb2
    let deltaR = float r1 - float r2
    let deltaG = float g1 - float g2
    let deltaB = float b1 - float b2
    sqrt ( deltaR * deltaR + deltaG * deltaG + deltaB * deltaB )

let calculateMatch (template:CharTemplate) (unknown:CharTemplate) : float =
    // for now assume the same size
    let scale (tBitmap:Bitmap) (sum:float) =
        sum / float ( tBitmap.Width * tBitmap.Height )

    let calcPixelDelta (tBitmap:Bitmap) (uBitmap:Bitmap) x y =
        let tColor = tBitmap.GetPixel( x, y ) |> colorToRgb
        let uColor = uBitmap.GetPixel( x, y ) |> colorToRgb
        calcDiff tColor uColor
    
    let calcLineDelta (tBitmap:Bitmap) (uBitmap:Bitmap) y =
        let width = tBitmap.Width
        [0 .. width - 1]
        |> List.map (fun x -> calcPixelDelta tBitmap uBitmap x y )
        |> List.sum
    
    let (CharTemplate tBitmap) = template
    let (CharTemplate uBitmap) = unknown
    let height = tBitmap.Height
    [0 .. height - 1]
    |> List.map (fun y -> calcLineDelta tBitmap uBitmap y )
    |> List.sum
    |> scale tBitmap

