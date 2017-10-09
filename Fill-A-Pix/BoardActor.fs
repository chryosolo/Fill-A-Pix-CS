module Fill_A_Pix.BoardActor

open Akka.FSharp
open Akka.Actor
open Akka.Routing
open System.Drawing
open GameTypes
open Messages
open Utility


let getCellCoords cell = (cell.Point.X, cell.Point.Y)

let processUpdateBoardCommand board command =
    match command with
    | UpdateCellValue (cell,newValue) ->
        let (x,y) = getCellCoords cell
        let cells = board.Cells |> Array2D.copy
        let cell' = cells.[x,y]
        cells.SetValue({cell' with Value=newValue},x,y)
        {board with Cells = cells}
    | UpdateClueState (cell,newState) ->
        match cell.Clue with
        | None -> failwith "Cannot update clue state if cell has no clue."
        | Some (clue,_) ->
            let (x,y) = getCellCoords cell
            let cells = board.Cells |> Array2D.copy
            let cell' = cells.[x,y]
            cells.SetValue({cell' with Clue=Some(clue,newState)},x,y)
            {board with Cells = cells}

let processUpdateBoardCommands board commands =
    commands
    |> List.fold processUpdateBoardCommand board


let actor (self:Actor<obj>) =
    let game = spawn self "game" GameActor.actor
    let recogConfig = [SpawnOption.Router(SmallestMailboxPool( 16 ))]
    let coord = self.Context.Parent
    let recog = spawnOpt self "recog" (RecognizerActor.actor self.Self) recogConfig

    // when ready we update board state until stopped
    let rec solving (state:BoardState) = actor {
        let! msg = self.Receive()
        match msg with
        // update board state directly to given new state
        | :? SetBoardStateMsg as msg' ->
            match msg' with
            | SetBoardState newState -> return! solving newState
        | :? StepMoveMsg -> game <! StepMoveFrom state
        // update board state by updating a cell's state or clue
        | :? FoundMoveMsg as msg' ->
            let state' =
                match msg' with
                | FoundEndOfGame -> state
                | FoundZeroClue list
                | FoundStartingClue list
                | FoundEnoughFilled list
                | FoundEnoughBlank list -> processUpdateBoardCommands state list
            let changed =
                state'.Cells
                |> Seq.cast<Cell>
                |> Seq.where( fun c -> c <> state.Cells.[c.Point.X,c.Point.Y] )
                |> Seq.toArray
            coord <! DrawCells changed
            return! solving state'
        | _ -> self.Unhandled msg
        return! solving state
    }

    // when bounded we search for clues and when all cells have been
    // matched, become ready
    let rec sized (cells:Cell[,]) = actor {
        let! msg = self.Receive()
        match msg with
        // forward to coord
        | :? CluesFinalizedMsg as msg' ->
            match msg' with
            | CluesFinalized cells ->
                coord <! msg
                let cx = Array2D.length1 cells
                let cy = Array2D.length2 cells
                let board = {Rows=cy; Cols=cx; Cells=cells}
                coord <! SetBoardState board
                return! solving board
        | :? FoundClueMsg as msg' ->
            match msg' with
            | FoundClue (CoordX x, CoordY y, clue) ->
                coord.Forward msg
                let cellClue = clue |> Option.map (fun c -> (c,Active))
                let cell = {Value=Unknown; Clue=cellClue; Point={X=x; Y=y}}
                cells.[x,y] <- cell
                let isStillFinding =
                    cells
                    |> Seq.cast<Cell>
                    |> Seq.exists (fun c -> c.Value=Init)
                if not isStillFinding then self.Self <! CluesFinalized cells
        // anything else is unhandled
        | _ -> self.Unhandled msg
        return! sized cells
    }

    let rec bounded (board:Pixels) = actor {
        let! msg = self.Receive()
        match msg with
        | :? FoundCellsMsg as msg' ->
            match msg' with
            | FoundCells (cols, rows) ->
                let (x,y) = cols.Length,rows.Length
                coord.Forward msg
                Array.allPairs cols rows
                |> Array.iter (fun ((cx, PixelX x0, PixelX x1), (cy, PixelY y0, PixelY y1)) ->
                    let rect = {Top=PixelY (y0+1); Bottom=PixelY (y1-1); Left=PixelX (x0+1); Right=PixelX (x1-1)}
                    let cell = board |> clipPixels rect
                    recog <! DetectClue (cx, cy, cell) )
                let cells = Array2D.init x y (fun x y -> {Value=Init; Clue=None; Point={X=x;Y=y}} )
                return! sized cells
        // anything else is unhandled
        | _ -> self.Unhandled msg
        return! bounded board
    }

    // when unbounded, we forward all FoundSide, and end with a FoundSize
    let rec unbounded (window:Pixels) = actor {
        let! msg = self.Receive()
        match msg with
        | :? FoundSideMsg -> coord.Forward msg
        // forward to coord, clip, tell recog to DetectCells and we become bounded
        | :? SidesFinalizedMsg as msg' ->
            match msg' with
            | SidesFinalized rect ->
                let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
                coord.Forward msg
                let board = window |> clipPixels rect
                recog <! DetectCells board
                return! bounded board
        // anything else is unhandled
        | _ -> self.Unhandled msg
        return! unbounded window
    }

    // initially, we expect only to receive a DetectBoard message
    let rec initial () = actor {
        let! msg = self.Receive()
        match msg with
        // DetectSides is sent to recog and we become unbounded
        | :? SetWindowMsg as msg' ->
            match msg' with
            | SetWindow window ->
                recog <! DetectSides window
                return! unbounded window
        // anything else is unhandled
        | _ -> self.Unhandled msg
        return! initial ()
    }
    initial ()
