module Fill_A_Pix.UiActor

open System
open Akka.FSharp
open Akka.Actor
open GameTypes
open Messages
open Fill_A_Pix_CS

let drawCell (form:FrmMain) cell =
    let cellValue = match cell.Value with
                    | Filled -> UiCellValue.Filled
                    | Blank -> UiCellValue.Empty
                    | _ -> UiCellValue.Unknown
    let (clueVal, clueState) =
        match cell.Clue with
        | None -> (Nullable(), UiClueState.Used)
        | Some (cv, Active) -> (Nullable(int cv), UiClueState.Active)
        | Some (cv, Used) -> (Nullable(int cv), UiClueState.Used)
    form.LogShowClue( cell.Point.X, cell.Point.Y, clueVal, clueState, cellValue )

let actor (form:FrmMain) (self:Actor<obj>) =
    let rec loop (coord:IActorRef option) = actor {
        let! msg = self.Receive()
        match msg with
        // UI has requested a new file be opened
        | :? UiImageOpen as uio ->
            // if coord exists, stop it
            coord |> Option.map (fun c -> self.Context.Stop( c ) ) |> ignore
            let newCoord = spawn (self.Context.System) "coordinator" CoordinatorActor.actor
            // inform coordinator of ui actor existence
            newCoord <! SetUi self.Self
            newCoord <! OpenBitmapFile (BitmapFile uio.Filename)
            return! loop (Some newCoord)
        | :? UiGameStep ->
            coord |> Option.map (fun coordRef -> coordRef <! StepMove) |> ignore
        // coordinator has requested UI show an image
        | :? ShowImageMsg as msg' ->
            match msg' with
            | ShowWindowImage (WindowBitmap bitmap) -> form.LogSetWindow( bitmap )
            | ShowBoardImage (BoardBitmap bitmap) -> form.LogShowImage( bitmap )
        // board sides have been found
        | :? FoundSideMsg as msg' ->
            match msg' with
            | FoundTopSide (PixelY py) -> form.LogDrawHLine( py )
            | FoundBottomSide (PixelY py) -> form.LogDrawHLine( py )
            | FoundLeftSide (PixelX px) -> form.LogDrawVLine( px )
            | FoundRightSide (PixelX px) -> form.LogDrawVLine( px )
        //| :? SidesFinalizedMsg  
        | :? FoundCellsMsg as msg' ->
            match msg' with
            | FoundCells (cols,rows) -> form.LogSizing(cols.Length, rows.Length)
        | :? FoundClueMsg as msg' ->
            match msg' with
            | FoundClue (CoordX cx, CoordY cy, Some clue) -> form.LogClueOverlay( cx, cy, int clue )
            | FoundClue (CoordX cx, CoordY cy, None) -> form.LogBlankOverlay( cx, cy )
        | :? CluesFinalizedMsg as msg' ->
            match msg' with
            | CluesFinalized cells ->
                form.LogClear();
                cells |> Seq.cast<Cell> |> Seq.iter (drawCell form)
        | :? SetBoardStateMsg as msg' ->
            match msg' with
            | SetBoardState state -> state.Cells |> Seq.cast<Cell> |> Seq.iter (drawCell form)
        | :? DrawCellsMsg as msg' ->
            match msg' with
            | DrawCells cells -> cells |> Array.iter (drawCell form)
        // Unhandled
        | _ -> self.Unhandled msg

        return! loop coord
    }

    loop None
