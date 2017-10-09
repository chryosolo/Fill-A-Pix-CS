module Fill_A_Pix.GameActor

open Akka.FSharp
open Akka.Actor
open System.Drawing
open GameTypes
open Messages


type UsableClue =
    | ZeroClue of Cell
    | StartingClue of Cell
    | EnoughFilled of Cell
    | EnoughBlank of Cell


let isOnBoard board point =
    point.Y >= 0
    && point.X >= 0
    && point.Y < board.Rows
    && point.X < board.Cols


let getCellNeighborPts board cell =
    let deltas = [|-1;0;1|]
    let isOnBoard = isOnBoard board
    Array.allPairs deltas deltas
    |> Array.map (fun (x,y) -> {X=cell.Point.X+x; Y=cell.Point.Y+y} )
    |> Array.where isOnBoard


let getCellNeighbors board cell =
    getCellNeighborPts board cell
    |> Array.map (fun p -> board.Cells.[p.X,p.Y])


let getCellFromBoard board point =
    board.Cells.[point.X,point.Y]


let (|IsZeroClue|_|) (_:Cell[],cell:Cell) =
    match cell.Clue with
    | Some (Clue.Zero,_) -> Some (ZeroClue cell)
    | _ -> None


let (|IsStartingClue|_|) (neighbors:Cell[],cell:Cell) =
    let total = neighbors.Length
    match cell.Clue with
    | Some (clue,_) when total = int clue -> Some (StartingClue cell)
    | _ -> None


let (|IsEnoughFilled|_|) (neighbors:Cell[],cell:Cell) =
    let filled = neighbors |> Array.where (fun n -> n.Value = Filled)
    match cell.Clue with
    | Some (clue,_) when filled.Length = (int clue) -> Some (EnoughFilled cell)
    | _ -> None


let (|IsEnoughBlank|_|) (neighbors:Cell[],cell:Cell) =
    let blank = neighbors |> Array.where (fun n -> n.Value = Blank)
    match cell.Clue with
    | Some (clue,_) when neighbors.Length - blank.Length = (int clue) ->
        Some (EnoughBlank cell)
    | _ -> None


let checkCell board cell =
    let neighbors = getCellNeighbors board cell
    match neighbors,cell with
    | IsZeroClue zero -> Some zero
    | IsStartingClue starting -> Some starting
    | IsEnoughBlank filled -> Some filled
    | IsEnoughFilled blank -> Some blank
    | _ -> None


let getActiveCluesFromBoard (board:BoardState) =
    board.Cells
    |> Seq.cast<Cell>
    |> Seq.where (fun cell ->
        match cell.Clue with
        | Some (_,Active) -> true
        | Some (_,Used) -> false
        | None -> false )


let getUpdateCommands board cell newValue =
    getCellNeighbors board cell
    |> Array.toList
    |> List.where (fun c -> c.Value = Unknown)
    |> List.map (fun c -> UpdateCellValue (c,newValue))
    |> List.append [UpdateClueState (cell,Used)]


let findMove board =
    let nextMove = getActiveCluesFromBoard board |> Seq.tryPick (checkCell board)
    let getCommands = getUpdateCommands board
    match nextMove with
    | Some (ZeroClue cell) -> FoundZeroClue (getCommands cell Blank)
    | Some (StartingClue cell) -> FoundStartingClue (getCommands cell Filled)
    | Some (EnoughFilled cell) -> FoundEnoughFilled (getCommands cell Blank)
    | Some (EnoughBlank cell) -> FoundEnoughBlank (getCommands cell Filled)
    | _ -> FoundEndOfGame


let actor (self:Actor<obj>) =
    let board = self.Context.Parent
    let rec loop () = actor {
        let! msg = self.Receive()
        match msg with
        | :? StepMoveFromMsg  as msg' ->
            match msg' with
            | StepMoveFrom state ->
                let move = findMove state
                board <! move
                return! loop ()
        | _ -> self.Unhandled msg
        return! loop ()
    }

    loop ()
