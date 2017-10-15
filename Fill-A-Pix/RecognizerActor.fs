module Fill_A_Pix.RecognizerActor

open Akka.FSharp
open Akka.Actor
open System.Drawing
open GameTypes
open Messages
open Fill_A_Pix.Utility


type MatchProbability = Clue * float
type ClueTemplate = { Clue: Clue option; Pixels: Pixels }


// Load the file with the given name from an image file and convert it to a Pixel array
let loadPixels name =
    let filename = sprintf "C:\\git\\MySandbox\\Fill-A-Pix\\Fill-A-Pix\\Numbers\\%s.png" name
    new Bitmap( Image.FromFile( filename ) )
    |> toPixels


// Turn the given clue into a template holding the correct Pixel array
let templateOf clue =
    let pixels = int clue |> sprintf "%i" |> loadPixels
    {Clue=Some clue; Pixels=pixels}


let blank = {Clue=None; Pixels=loadPixels "blank"}
let clueTemplates =
    [ 0 .. 9 ]
    |> List.map (fun i -> templateOf (enum<Clue>( i ) ) )
    |> List.append [blank]


let calcDiff (color1:Color) (color2:Color) =
    let deltaR = float color1.R - float color2.R
    let deltaG = float color1.G - float color2.G
    let deltaB = float color1.B - float color2.B
    sqrt ( deltaR*deltaR + deltaG*deltaG + deltaB*deltaB )


// compare all pixels, accumulating the amount of difference
let calculateMatch (clue:Pixels) (unknown:Pixels) : float =
    let (width, height) = getDimensions clue
    let scaled sum = sum / float (width * height)

    Array.allPairs [|0 .. width - 1|] [|0 .. height - 1|]
    |> Array.map (fun (x,y) -> calcDiff clue.[x,y] unknown.[x,y])
    |> Array.sum
    |> scaled


// match the given unknown pixels against all templates and return the clue with
// the smallest difference
let matchClue (unknown:Pixels) =
    clueTemplates
    |> Seq.map (fun ct -> ( ct.Clue, calculateMatch (ct.Pixels) unknown ))
    |> Seq.sortBy snd
    |> Seq.head


// return if two colors are equal
let isRgbMatch (color1:Color) (color2:Color) =
    color1.R = color2.R && color1.G = color2.G && color1.B = color2.B


// generic detection algorithm -- get color of all pixel offsets and whether they match target
// and reduce as specified (and/or)
let detector reducer color (offsets:(int*int)[]) (pixels:Pixels) baseX baseY =
    let isRgbMatchForTargetColor = isRgbMatch color
    offsets
    |> Array.map (fun (dx,dy) -> pixels.[baseX+dx, baseY+dy])
    |> Array.map isRgbMatchForTargetColor
    |> Array.reduce reducer

// specific detectors
let getLeftBorder = detector (||) Color.White [|(1, 0); (1, -1)|]
let getRightBorder = detector (||) Color.White [|(-1, 0); (-1, -1)|]
let getBottomBorder = detector (||) Color.White [|(0, -1); (-1, -1)|]
let getTopLeftCorner = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(1,0); (1,-1)|]
let getColumn = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(0,0); (0,1)|]
let getRow = detector (&&) (Color.FromArgb( 132, 132, 132 ) ) [|(0,0); (1,0)|]


let findBoard window boardRef =
    let (width,height) = getDimensions window
    let halfWidth = width / 2
    let halfHeight = height / 2

    // we keep leftX an int because we use it below as int
    let leftX =
        seq [ 0 .. halfWidth ] // work from left to center
        |> Seq.skipWhile (fun x -> not ( getLeftBorder window x halfHeight )) 
        |> Seq.head
    boardRef <! FoundLeftSide (PixelX leftX)
    // remaining coords get Pixel-ated because they're not used elsewhere
    let bottomY =
        seq [ height-1 .. -1 .. halfHeight ] // work from bottom to center
        |> Seq.skipWhile (fun y -> not ( getBottomBorder window halfWidth y ))
        |> Seq.head
        |> PixelY
    boardRef <! FoundBottomSide bottomY
    let rightX =
        seq [ width-1 .. -1 .. halfWidth ] // work from right to center
        |> Seq.skipWhile (fun x -> not ( getRightBorder window x halfHeight ))
        |> Seq.head
        |> PixelX
    boardRef <! FoundRightSide rightX
    let topY =
        seq [ halfHeight .. -1 .. 0 ] // work from center to top
        |> Seq.skipWhile (fun y -> not ( getTopLeftCorner window leftX y ))
        |> Seq.head
        |> PixelY
    boardRef <! FoundTopSide topY
    let rect = {Top=topY; Bottom=bottomY; Left=(PixelX leftX); Right=rightX}
    boardRef <! SidesFinalized rect


let getCellBoundaries (pixels:Pixels) =
    let (width,height) = getDimensions pixels

    // find grid column pixel boundaries
    let colBoundaries =
        [| 0 .. width-1|]
        |> Array.where (fun x -> getColumn pixels x 0) // x's with vertical lines
        |> Array.pairwise
        |> Array.mapi (fun idx (x0, x1) -> CoordX idx, PixelX x0, PixelX x1)
    let rowBoundaries =
        [| 0 .. height-1|]
        |> Array.where(fun y -> getRow pixels 0 y) // y's with horizontal lines
        |> Array.pairwise
        |> Array.mapi (fun idx (y0, y1) -> CoordY idx, PixelY y0, PixelY y1)
    // find grid coords and cell clues
    (colBoundaries,rowBoundaries)


let actor (boardRef:IActorRef) (self:Actor<obj>) =
    let rec loop () = actor {
        let! msg = self.Receive()
        match msg with
        | :? DetectSidesMsg as msg' ->
            match msg' with
            | DetectSides window -> findBoard window boardRef
        | :? DetectCellsMsg as msg' ->
            match msg' with
            | DetectCells board ->
                let boundaries = getCellBoundaries board
                boardRef <! FoundCells boundaries
        | :? DetectClueMsg as msg' ->
            match msg' with
            | DetectClue (cx, cy, cell) ->
                let (clue,delta) = matchClue cell
                boardRef <! FoundClue (cx,cy,clue)
        // anything else is unhandled
        | _ -> self.Unhandled msg
        return! loop ()
    }
    loop ()
