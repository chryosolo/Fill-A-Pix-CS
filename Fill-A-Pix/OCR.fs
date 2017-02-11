module OCR

open System.Drawing
open Game

type MatchProbability = Clue * float

type ClueTemplate = { Clue: Clue; Bitmap: Bitmap }

let clueToString =
    function
    | Blank -> " "
    | Given GivenClue.Zero -> "0"
    | Given GivenClue.One -> "1"
    | Given GivenClue.Two -> "2"
    | Given GivenClue.Three -> "3"
    | Given GivenClue.Four -> "4"
    | Given GivenClue.Five -> "5"
    | Given GivenClue.Six -> "6"
    | Given GivenClue.Seven -> "7"
    | Given GivenClue.Eight -> "8"
    | Given GivenClue.Nine -> "9"
    | _ -> "X"
                
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

let blankClue = { Clue = Blank; Bitmap = loadBitmap "blank" }
let zeroClue = { Clue = Given GivenClue.Zero; Bitmap = loadBitmap "0" }
let oneClue = { Clue = Given GivenClue.One; Bitmap = loadBitmap "1" }
let twoClue = { Clue = Given GivenClue.Two; Bitmap = loadBitmap "2" }
let threeClue = { Clue = Given GivenClue.Three; Bitmap = loadBitmap "3" }
let fourClue = { Clue = Given GivenClue.Four; Bitmap = loadBitmap "4" }
let fiveClue = { Clue = Given GivenClue.Five; Bitmap = loadBitmap "5" }
let sixClue = { Clue = Given GivenClue.Six; Bitmap = loadBitmap "6" }
let sevenClue = { Clue = Given GivenClue.Seven; Bitmap = loadBitmap "7" }
let eightClue = { Clue = Given GivenClue.Eight; Bitmap = loadBitmap "8" }
let nineClue = { Clue = Given GivenClue.Nine; Bitmap = loadBitmap "9" }
let clueTemplates = [| blankClue; zeroClue; oneClue; twoClue; threeClue; fourClue; fiveClue; sixClue; sevenClue; eightClue; nineClue |]

let matchClue (unknown:Bitmap) =
    clueTemplates
    |> Array.map ( fun clueTemp -> ( clueTemp.Clue, calculateMatch (clueTemp.Bitmap) unknown ) )
    |> Array.sortBy ( fun clueMatch -> snd clueMatch )
    |> Array.head

let rgbMatch (color1:Color) (color2:Color) =
    color1.R = color2.R && color1.G = color2.G && color1.B = color2.B

// generic detection algorithm -- get color of all pixel offsets and whether they match target
// and reduce using the specified matcher (and/or)
let detector (matcher:bool->bool->bool) (color:Color) (offsets:(int*int)[]) (window:Bitmap) x y =
    let matchTarget = rgbMatch color
    let matches =
        offsets
        |> Array.map (fun (dx,dy) -> window.GetPixel( x+dx, y+dy ) )
        |> Array.map matchTarget
    matches
    |> Array.reduce matcher

let detectLeftBoundary = detector (||) Color.White [|(1, 0); (1, -1)|]
let detectRightBoundary = detector (||) Color.White [|(-1, 0); (-1, -1)|]
let detectBottomBoundary = detector (||) Color.White [|(0, -1); (-1, -1)|]
let detectTopLeftCorner = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(1,0); (1,-1)|]
let detectColumn = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(0,0); (0,1)|]
let detectRow = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(0,0); (1,0)|]

let findBoard (window:Bitmap) : Bitmap =
    let halfHeight = window.Height/2
    let halfWidth = window.Width/2
    let leftBoundaryX =
        Seq.skipWhile (fun x -> not ( detectLeftBoundary window x halfHeight )) (seq [ 0 .. halfWidth ])
        |> Seq.head 
    let bottomBoundaryY =
        Seq.skipWhile (fun y -> not ( detectBottomBoundary window halfWidth y ))
                      (seq [ window.Height-1 .. -1 .. halfHeight ])
        |> Seq.head
    let rightBoundaryX =
        Seq.skipWhile (fun x -> not ( detectRightBoundary window x halfHeight ))
                      (seq [ window.Width-1 .. -1 .. halfWidth ])
        |> Seq.head 
    let topBoundaryY =
        Seq.skipWhile (fun y -> not ( detectTopLeftCorner window leftBoundaryX y ))
                      (seq [ halfHeight .. -1 .. 0 ])
        |> Seq.head

    let boardRect = new Rectangle( leftBoundaryX, topBoundaryY,
                                   rightBoundaryX-leftBoundaryX+1, bottomBoundaryY-topBoundaryY+1 )
    window.Clone( boardRect, window.PixelFormat )

let findClues (board:Bitmap) =
    // find grid column pixel boundaries
    let colBoundaries =
        [| 0 .. board.Width-1 |]
        |> Array.where (fun x -> detectColumn board x 0) // x's with vertical lines
        |> Array.pairwise
    // find grid row indices and pixel boundaries
    let rowBoundaries =
        [| 0 .. board.Height-1 |]
        |> Array.where (fun y -> detectRow board 0 y)
        |> Array.pairwise
    // find grid coords and cell clues
    Array2D.init (colBoundaries.Length) (rowBoundaries.Length)
        (fun x y ->
            let (x0,x1) = colBoundaries.[x]
            let (y0,y1) = rowBoundaries.[y]
            let rect = new Rectangle( x0+1, y0+1, x1-x0-1, y1-y0-1 )
            let bitmap = board.Clone( rect, board.PixelFormat )
            let (clue,_) = matchClue bitmap
            clue )


