module Game

type CellValue =
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

type StatedClue = (Clue*ClueState) option

type Point = {X:int; Y:int}

type Cell = {Value:CellValue; Clue:StatedClue; Point:Point}

type Board = {Rows:int; Cols:int; Cells:Cell[,]}

type FoundUsableClue =
    | BeginningOfGame
    | NothingFound
    | StartingClue of Cell
    | EnoughFilled of Cell
    | EnoughBlank of Cell

type UpdateBoardCommand =
    | UpdateCellValue of Cell*CellValue
    | UpdateClueState of Cell*ClueState

type GameState = Board * (FoundUsableClue list)

let printCell cell =
    let (surroundFst, surroundSnd) =
        match cell.Value with
        | Unknown -> ("(", ")")
        | Filled -> ("«", "»")
        | Blank -> ("‹", "›")
    let clue =
        match cell.Clue with
        | Some (clue,_) -> (int clue).ToString()
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
    |> printfn "%s"

let isOnBoard board point =
    point.Y >= 0 && point.X >= 0
    && point.Y < board.Rows && point.X < board.Cols

let getCellNeighborPts board cell =
    let deltas = [|-1;0;1|]
    let isOnBoard = isOnBoard board
    Array.allPairs deltas deltas
    |> Array.map (fun (x,y) -> {X=cell.Point.X+x; Y=cell.Point.Y+y} )
    |> Array.where isOnBoard

let printLastClue clues =
    let lastClue = List.head clues
    match lastClue with
    | BeginningOfGame -> printfn "Beginning of Game"
    | NothingFound -> printfn "Nothing Found -- game ends"
    | StartingClue cell
    | EnoughFilled cell
    | EnoughBlank cell ->
        match cell.Clue with
        | Some (clue,state) ->
            printfn "Found %s of %d at %d %d" (lastClue.ToString()) (int clue) (cell.Point.X) (cell.Point.Y)
        | None -> printfn "Found starting clue, but clue is invalid... :("
 
let printState (board,clues) =
    printLastClue clues
    printBoard board

let getCellNeighbors board cell =
    getCellNeighborPts board cell
    |> Array.map (fun p -> board.Cells.[p.Y,p.X])

let getCellFromBoard board point =
    board.Cells.[point.Y,point.X]

let (|FoundStartingClue|_|) (cells:Cell[],cell:Cell) =
    let total = cells.Length
    match cell.Clue with
    | Some (Clue.Zero,_) -> StartingClue cell |> Some
    | Some (clue,_) when total = int clue -> StartingClue cell |> Some
    | _ -> None

let (|FoundAllOfState|_|) state (cells:Cell[],cell:Cell) =
    let filled = cells |> Array.where (fun cell -> cell.Value = state)
    match cell.Clue with
    | Some (clue,_) when filled.Length = int clue -> Some true
    | _ -> None

let (|FoundEnoughFilled|_|) (cells:Cell[],cell:Cell) =
    let filled = cells |> Array.where (fun cell -> cell.Value = Filled)
    let unknown = cells |> Array.where (fun cell -> cell.Value = Unknown)
    match cell.Clue with
    | Some (clue,_) when filled.Length = (int clue) -> EnoughFilled cell |> Some
    | _ -> None

let (|FoundEnoughBlank|_|) (cells:Cell[],cell:Cell) =
    let blank = cells |> Array.where (fun cell -> cell.Value = Blank)
    let unknown = cells |> Array.where (fun cell -> cell.Value = Unknown)
    match cell.Clue with
    | Some (clue,_) when cells.Length - blank.Length = (int clue) -> EnoughBlank cell |> Some
    | _ -> None

let checkCell board cell =
    let cells = getCellNeighbors board cell
    match cells,cell with
    | FoundStartingClue starting -> Some starting
    | FoundEnoughFilled filled -> Some filled
    | FoundEnoughBlank blank -> Some blank
    | _ -> None

let getCluesFromBoard (board:Board) =
    board.Cells
    |> Seq.cast<Cell>
    |> Seq.where (fun cell ->
        match cell.Clue with
        | Some (clue,Active) -> true
        | Some (clue,Used) -> false
        | None -> false )

let getUpdateCellValuesEvent filter board cell newValue =
    getCellNeighbors board cell
    |> Array.toList
    |> List.where filter
    |> List.map (fun cell -> UpdateCellValue (cell,newValue))
    |> List.append [UpdateClueState (cell,Used)]

let getUpdateValuesForStart = getUpdateCellValuesEvent (fun _ -> true )
let getUpdateValuesForEnough = getUpdateCellValuesEvent (fun cell -> cell.Value = Unknown )

let getBoardChangeCommands board (clue:FoundUsableClue) =
    match clue with
    | BeginningOfGame -> []
    | NothingFound -> []
    | StartingClue cell ->
        match cell.Clue with
        | Some ( Clue.Zero, Active ) -> getUpdateValuesForStart board cell Blank
        | Some ( _, Active ) -> getUpdateValuesForStart board cell Filled
        | Some ( _, Used ) -> failwith "Cannot process a used clue"
        | None -> failwith "Cannot process a cell with no clue"
    | EnoughFilled cell -> getUpdateValuesForEnough board cell Blank
    | EnoughBlank cell -> getUpdateValuesForEnough board cell Filled

let processUpdateBoardCommand board command =
    match command with
    | UpdateCellValue (cell,newValue) ->
        let cells = board.Cells |> Array2D.copy
        let cell' = cells.[cell.Point.Y,cell.Point.X]
        cells.SetValue({cell' with Value=newValue},cell.Point.Y,cell.Point.X)
        {board with Cells = cells}
    | UpdateClueState (cell,newState) ->
        match cell.Clue with
        | None -> failwith "Cannot update clue state if cell has no clue."
        | Some (clue,_) ->
            let cells = board.Cells |> Array2D.copy
            let cell' = cells.[cell.Point.Y,cell.Point.X]
            cells.SetValue({cell' with Clue=Some(clue,newState)},cell.Point.Y,cell.Point.X)
            {board with Cells = cells}

let processUpdateBoardCommands board commands =
    commands
    |> List.fold processUpdateBoardCommand board

let advanceState state =
    let (board,oldClues) = state
    let nextPotentialClue = getCluesFromBoard board |> Seq.tryPick (checkCell board)
    match nextPotentialClue with
    | None -> (board,NothingFound::oldClues)
    | Some nextClue ->
        let board' =
            getBoardChangeCommands board nextClue
            |> processUpdateBoardCommands board
        (board',nextClue::oldClues)

let solveBoard board =
    let mutable currentState = (board,[BeginningOfGame])
    while (snd currentState |> List.head <> NothingFound) do
        let nextState = advanceState currentState
        do printState nextState
        currentState <- nextState
