module OCR

open System.Drawing

type Char =
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5
    | Six = 6
    | Seven = 7
    | Eight = 8
    | Nine = 9

type BlankOrChar = 
    | Blank
    | Filled of Char

type CharTemplate = { Char:BlankOrChar; Bitmap:Bitmap }

type MatchProbability = BlankOrChar * float

let calcDiff (color1:Color) (color2:Color) =
    let deltaR = float color1.R - float color2.R
    let deltaG = float color1.G - float color2.G
    let deltaB = float color1.B - float color2.B
    sqrt ( deltaR * deltaR + deltaG * deltaG + deltaB * deltaB )

let calculateMatch (template:Bitmap) (unknown:Bitmap) : float =
    let scale (tBitmap:Bitmap) (sum:float) =
        sum / float ( tBitmap.Width * tBitmap.Height )

    let calcPixelDelta (tBitmap:Bitmap) (uBitmap:Bitmap) x y =
        let tColor = tBitmap.GetPixel( x, y )
        let uColor = uBitmap.GetPixel( x, y )
        calcDiff tColor uColor
    
    let calcLineDelta (tBitmap:Bitmap) (uBitmap:Bitmap) y =
        let width = tBitmap.Width
        [0 .. width - 1]
        |> List.map (fun x -> calcPixelDelta tBitmap uBitmap x y )
        |> List.sum
    
    let height = template.Height
    [0 .. height - 1]
    |> List.map (fun y -> calcLineDelta template unknown y )
    |> List.sum
    |> scale template

let loadBitmap name =
    let filename = sprintf "C:\\git\\Sandbox\\Fill-A-Pix\\Fill-A-Pix\\Numbers\\%s.png" name
    new Bitmap( Image.FromFile( filename ) )

let blankChar = { Char = Blank; Bitmap = loadBitmap "blank" }
let zeroChar = { Char = Filled Char.Zero; Bitmap = loadBitmap "0" }
let oneChar = { Char = Filled Char.One; Bitmap = loadBitmap "1" }
let twoChar = { Char = Filled Char.Two; Bitmap = loadBitmap "2" }
let threeChar = { Char = Filled Char.Three; Bitmap = loadBitmap "3" }
let fourChar = { Char = Filled Char.Four; Bitmap = loadBitmap "4" }
let fiveChar = { Char = Filled Char.Five; Bitmap = loadBitmap "5" }
let sixChar = { Char = Filled Char.Six; Bitmap = loadBitmap "6" }
let sevenChar = { Char = Filled Char.Seven; Bitmap = loadBitmap "7" }
let eightChar = { Char = Filled Char.Eight; Bitmap = loadBitmap "8" }
let nineChar = { Char = Filled Char.Nine; Bitmap = loadBitmap "9" }
let charTemplates = [| blankChar; zeroChar; oneChar; twoChar; threeChar; fourChar; fiveChar; sixChar; sevenChar; eightChar; nineChar |]

let getChar (unknown:Bitmap) =
    charTemplates
    |> Array.map ( fun charTemp -> ( charTemp.Char, calculateMatch (charTemp.Bitmap) unknown ) )
    |> Array.sortBy ( fun charMatch -> snd charMatch )
    |> Array.take 3

let rgbMatch (color1:Color) (color2:Color) =
    color1.R = color2.R && color1.G = color2.G && color1.B = color2.B

// from middle of left side, call with increasing X -- returns true if right or above-right pixel is white
let detectLeftBoundary (window:Bitmap) x y =
    let matchTarget = rgbMatch Color.White
    let right = window.GetPixel( x+1, y )
    let rightAbove = window.GetPixel( x+1, y-1 )
    matchTarget right || matchTarget rightAbove

// from middle of right side, call with decreasing X -- returns true if left or above-left pixel is white
let detectRightBoundary (window:Bitmap) x y =
    let matchTarget = rgbMatch Color.White
    let left = window.GetPixel( x-1, y )
    let leftAbove = window.GetPixel( x-1, y-1 )
    matchTarget left || matchTarget leftAbove

// from middle of bottom, call with decreasing Y -- returns true if above or above-left pixel is white
let detectBottomBoundary (window:Bitmap) x y =
    let matchTarget = rgbMatch Color.White
    let above = window.GetPixel( x, y-1 )
    let aboveLeft = window.GetPixel( x-1, y-1 )
    matchTarget above || matchTarget aboveLeft

// from found left boundary (middle Y), call with decreasing Y -- returns true if right AND above-right pixel are gray
let detectTopLeftCorner (window:Bitmap) x y =
    let matchTarget = rgbMatch ( Color.FromArgb( 132, 132, 132 ) )
    let right = window.GetPixel( x+1, y )
    let rightAbove = window.GetPixel( x+1, y-1 )
    matchTarget right && matchTarget rightAbove

let findBoard (window:Bitmap) : Bitmap =
    let halfHeight = window.Height/2
    let halfWidth = window.Width/2
    let leftBoundaryX =
        Seq.skipWhile (fun x -> not ( detectLeftBoundary window x halfHeight ) ) (seq [ 0 .. halfWidth ])
        |> Seq.head 
    let bottomBoundaryY =
        Seq.skipWhile (fun y -> not ( detectBottomBoundary window halfWidth y ) )
                      (seq [ window.Height-1 .. -1 .. halfHeight ])
        |> Seq.head
    let rightBoundaryX =
        Seq.skipWhile (fun x -> not ( detectRightBoundary window x halfHeight ) )
                      (seq [ window.Width-1 .. -1 .. halfWidth ])
        |> Seq.head 
    let topBoundaryY =
        Seq.skipWhile (fun y -> not ( detectTopLeftCorner window leftBoundaryX y ) )
                      (seq [ halfHeight .. -1 .. 0 ] )
        |> Seq.head

    let boardRect = new Rectangle( leftBoundaryX, topBoundaryY,
                                   rightBoundaryX-leftBoundaryX+1, bottomBoundaryY-topBoundaryY+1 )
    let board = window.Clone( boardRect, window.PixelFormat )
    board.Save( "c:\\git\\Sandbox\\Fill-A-Pix\\board.png" )
    board
