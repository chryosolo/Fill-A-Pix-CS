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

let isOnBoard board point =
    point.Y >= 0 && point.X >= 0
    && point.Y < board.Rows && point.X < board.Cols

let addPoints pnt1 pnt2 =
    {Y=pnt1.Y + pnt2.Y; X=pnt1.X + pnt2.X}

let getCellNeighborPts board point =
    let deltas = [|-1;0;1|]
    let isOnBoard = isOnBoard board
    Array.allPairs deltas deltas
    |> Array.map (fun (r,c) -> addPoints point {X=c;Y=r})
    |> Array.where isOnBoard

let getCellNeighbors board point =
    getCellNeighborPts board point
    |> Array.map (fun p -> board.Cells.[p.Y,p.X])

let getCellFromBoard board point =
    board.Cells.[point.Y,point.X]

let (|FoundStartingClue|_|) (cells:Cell[],cell:Cell) =
    let total = cells.Length
    match cell.Clue with
    | Some Clue.Zero -> StartingClue cell |> Some
    | Some clue when total = int clue -> StartingClue cell |> Some
    | _ -> None

let (|FoundAllOfState|_|) state (cells:Cell[],cell:Cell) =
    let filled = cells |> Array.where (fun cell -> cell.State = state)
    match cell.Clue with
    | Some clue when filled.Length = int clue -> Some true
    | _ -> None

let (|FoundEnoughFilled|_|) (cells:Cell[],cell:Cell) =
    let filled = cells |> Array.where (fun cell -> cell.State = Filled)
    match cell.Clue with
    | Some clue when filled.Length = int clue -> EnoughFilled cell |> Some
    | _ -> None

let (|FoundEnoughBlank|_|) (cells:Cell[],cell:Cell) =
    let blank = cells |> Array.where (fun cell -> cell.State = Blank)
    match cell.Clue with
    | Some clue when blank.Length = int clue -> EnoughBlank cell |> Some
    | _ -> None

let checkCell board point =
    let cells = getCellNeighbors board point
    let cell = getCellFromBoard board point
    match cells,cell with
    | FoundStartingClue starting -> Some starting
    | FoundEnoughFilled filled -> Some filled
    | FoundEnoughBlank blank -> Some blank
    | _ -> None