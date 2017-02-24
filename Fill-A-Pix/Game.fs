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

type Point = {X:int; Y:int}

type Cell = {State:CellState; Clue:Clue; ClueState:ClueState; Point:Point}

type Board = {Rows:int; Cols:int; Cells:Cell[,]}

type Event =
    | NothingFound
    | StartingClue of Cell
    | EnoughFilled of Cell
    | EnoughBlank of Cell
    | UpdateCellStates of (Cell*CellState) list
    | UpdateClueState of Cell*ClueState

type CellStateBreakdown = (CellState*Cell[])[]

let printCell cell =
    let (surroundFst, surroundSnd) =
        match cell.State with
        | Unknown -> ("(", ")")
        | Filled -> ("«", "»")
        | Empty -> ("‹", "›")
    let clue =
        match cell.Clue with
        | Blank -> ' '
        | Given given -> (int given).ToString().ToCharArray() |> Array.head
    sprintf "%s%c%s" surroundFst clue surroundSnd

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
