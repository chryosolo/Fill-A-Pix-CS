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

type Match = BlankOrChar * float

let calcDiff (color1:Color) (color2:Color) =
    let deltaR = float color1.R - float color2.R
    let deltaG = float color1.G - float color2.G
    let deltaB = float color1.B - float color2.B
    sqrt ( deltaR * deltaR + deltaG * deltaG + deltaB * deltaB )

let calculateMatch (template:Bitmap) (unknown:Bitmap) : float =
    // for now assume the same size
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





