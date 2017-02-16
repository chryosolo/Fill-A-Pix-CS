module Game

type CellState =
    | Unknown
    | Filled
    | Empty

type ClueState =
    | Active
    | Used

type GivenClue =
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

type Clue = 
    | Blank
    | Given of GivenClue

type Cell = {State:CellState; Clue:Clue; ClueState:ClueState}

type Board = {Rows:int; Cols:int; Cells:Cell[,]}

type Point = {Row:int; Col:int}

type Event =
    | FoundStart of Point
    | EnoughFilled of Point
    | EnoughBlank of Point

let printCell cell =
    let surround =
        match cell.State with
        | Unknown -> '?'
        | Filled -> 'X'
        | Empty -> '.'
    let clue =
        match cell.Clue with
        | Blank -> ' '
        | Given given -> (int given).ToString().ToCharArray() |> Array.head
    sprintf "[%c%c%c]" surround clue surround

let printRow board rowIdx =
    let colIdx = [ 0 .. board.Cols-1 ]
    colIdx
    |> List.map (fun col -> board.Cells.[rowIdx,col])
    |> List.map printCell
    |> List.reduce (+)

let printBoard board =
    let rowIdx = [ 0 .. board.Rows-1 ]
    rowIdx
    |> List.map (fun row -> printRow board row)
    |> List.reduce (fun row1 row2 -> sprintf "%s\r\n%s" row1 row2)
