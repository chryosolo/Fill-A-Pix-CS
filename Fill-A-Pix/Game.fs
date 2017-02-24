module Game

type CellState =
    | Unknown
    | Filled
    | Blank

type ClueState =
    | Active
    | Used

type Clue =
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

type Point = {X:int; Y:int}

type Cell = {State:CellState; Clue:Clue option; ClueState:ClueState; Point:Point}

type Board = {Rows:int; Cols:int; Cells:Cell[,]}

type Event =
    | NothingFound
    | StartingClue of Cell
    | EnoughFilled of Cell
    | EnoughBlank of Cell
    | UpdateCellStates of (Cell*CellState) list
    | UpdateClueState of Cell*ClueState

let printCell cell =
    let (surroundFst, surroundSnd) =
        match cell.State with
        | Unknown -> ("(", ")")
        | Filled -> ("«", "»")
        | Blank -> ("‹", "›")
    let clue =
        match cell.Clue with
        | Some clue -> (int clue).ToString()
        | None -> " "
    sprintf "%s%s%s" surroundFst clue surroundSnd

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
