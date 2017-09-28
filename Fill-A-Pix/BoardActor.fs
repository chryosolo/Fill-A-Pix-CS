module Fill_A_Pix.BoardActor

open Akka.FSharp
open Akka.Actor
open Akka.Routing
open System.Drawing
open GameTypes
open Messages
open Utility


let stepBoard (board:BoardState) cell (stepFunc:Cell->Cell) =
    let (x,y) = cell.Point.X,cell.Point.Y
    let cells = board.Cells |> Array2D.copy
    let oldCell = cells.[y,x]
    let newCell = oldCell |> stepFunc
    cells.SetValue(newCell,y,x)
    {board with Cells=cells}


let actor (self:Actor<obj>) =
    let recogConfig = [SpawnOption.Router(SmallestMailboxPool( 8 ))]
    let coord = self.Context.Parent
    let recog = spawnOpt self "recog" (RecognizerActor.actor self.Self) recogConfig

    // when ready we update board state until stopped
    let rec ready (board:BoardState) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log -> coord.Forward msg
        // update board state directly to given new state
        | :? SetBoardStateMsg as msg' ->
            match msg' with
            | SetBoardState newState ->
                coord <! (self, "ready.SetBoardState")
                return! ready newState
        // update board state by updating a cell's state or clue
        | :? UpdateBoardStateMsg as msg' ->
            match msg' with
            | UpdateCellState (cell,state) ->
                coord <! (self, "ready.UpdateCellState")
                let board' = stepBoard board cell (fun c -> {c with Value=state})
                return! ready board'
            | UpdateClueState (cell,state) ->
                coord <! (self, "ready.UpdateClueState")
                let newClue = match cell.Clue with
                              | None -> None
                              | Some (clue,_) -> Some (clue,state)
                let board' = stepBoard board cell (fun c -> {c with Clue=newClue})
                return! ready board'
        // anything else is unhandled
        | _ ->
            coord <! (self, sprintf "ready.Unhandled: %A" msg)
            self.Unhandled msg
        return! ready board
    }

    // when bounded we search for clues and when all cells have been
    // matched, become ready
    let rec sized (cells:Cell[,]) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log -> coord.Forward msg
        // forward to coord
        | :? FoundSixMsg -> coord.Forward msg
        | :? CluesFinalizedMsg as msg' ->
            match msg' with
            | CluesFinalized cells ->
                coord <! (self, "sized.CluesFinalized")
                coord <! msg
                let cx = Array2D.length1 cells
                let cy = Array2D.length2 cells
                let board = {Rows=cy; Cols=cx; Cells=cells}
                coord <! SetBoardState board
                return! ready board
        | :? FoundClueMsg as msg' ->
            match msg' with
            | FoundClue (CoordX x, CoordY y, clue) ->
                coord <! (self, sprintf "sized.FoundClue x:%i, y:%i, clue:%A" x y clue)
                coord.Forward msg
                let cellClue = clue |> Option.map (fun c -> (c,Active))
                let cell = {Value=Unknown; Clue=cellClue; Point={X=x; Y=y}}
                cells.[x,y] <- cell
                let isStillFinding =
                    cells
                    |> Seq.cast<Cell>
                    |> Seq.exists (fun {Value=v; Clue=_; Point=_} -> v = Init)
                if not isStillFinding then self.Self <! CluesFinalized cells
        // anything else is unhandled
        | _ ->
            coord <! (self, sprintf "sized.Unhandled: %A" msg)
            self.Unhandled msg
        return! sized cells
    }

    let rec bounded (board:Pixels) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log -> coord.Forward msg
        | :? FoundCellsMsg as msg' ->
            match msg' with
            | FoundCells (cols, rows) ->
                let (x,y) = cols.Length,rows.Length
                coord <! (self, sprintf "bounded.FoundCells %i by %i" x y)
                coord.Forward msg
                Array.allPairs cols rows
                |> Array.iter (fun ((cx, PixelX x0, PixelX x1), (cy, PixelY y0, PixelY y1)) ->
                    let rect = {Top=PixelY (y0+1); Bottom=PixelY (y1-1); Left=PixelX (x0+1); Right=PixelX (x1-1)}
                    let cell = board |> clipPixels rect
                    recog <! DetectClue (cx, cy, cell) )
                let cells = Array2D.init x y (fun x y -> {Value=Init; Clue=None; Point={X=x;Y=y}} )
                return! sized cells
        // anything else is unhandled
        | _ ->
            coord <! (self, sprintf "bounded.Unhandled: %A" msg)
            self.Unhandled msg
        return! bounded board
    }

    // when unbounded, we forward all FoundSide, and end with a FoundSize
    let rec unbounded (window:Pixels) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log -> coord.Forward msg
        | :? FoundSideMsg ->
            coord <! (self, sprintf "unbounded.FoundSideMsg %A" msg)
            coord.Forward msg
        // forward to coord, clip, tell recog to DetectCells and we become bounded
        | :? SidesFinalizedMsg as msg' ->
            match msg' with
            | SidesFinalized rect ->
                let {Top=(PixelY t);Bottom=(PixelY b);Left=(PixelX l);Right=(PixelX r)} = rect
                coord <! (self, sprintf "unbounded.SidesFinalized t:%i b:%i l:%i r:%i" t b l r )
                coord.Forward msg
                let board = window |> clipPixels rect
                recog <! DetectCells board
                return! bounded board
        // anything else is unhandled
        | _ ->
            coord <! (self, sprintf "unbounded.Unhandled: %A" msg)
            self.Unhandled msg
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
                coord <! (self, "initial.SetWindow")
                recog <! DetectSides window
                return! unbounded window
        // anything else is unhandled
        | _ ->
            coord <! (self, sprintf "initial.Unhandled: %A" msg)
            self.Unhandled msg
        return! initial ()
    }
    initial ()
