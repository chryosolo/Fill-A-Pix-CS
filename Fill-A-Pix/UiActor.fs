module Fill_A_Pix.UiActor

open Akka.FSharp
open Akka.Actor
open GameTypes
open Messages
open Fill_A_Pix_UI
open Fill_A_Pix.Utility


let actor (form:FrmMain) (self:Actor<obj>) =
    let rec loop (coord:IActorRef option) = actor {
        let! msg = self.Receive()
        let sender = self.Sender()
        match msg with
        // UI has requested a new file be opened
        | :? UiImageOpen as uio ->
            // if coord exists, stop it
            coord |> Option.map (fun c -> self.Context.Stop( c ) ) |> ignore
            let newCoord = spawn (self.Context.System) "coordinator" CoordinatorActor.actor
            // inform coordinator of ui actor existence
            newCoord <! SetUi self.Self
            newCoord <! (self, sprintf "UiImageOpen %s" uio.Filename)
            newCoord <! OpenBitmapFile (BitmapFile uio.Filename)
            return! loop (Some newCoord)
        | :? UiGameStep as ugs ->
            coord |> Option.map (fun coordRef -> coordRef <! StepMove) |> ignore
        // coordinator has requested UI show an image
        | :? ShowImageMsg as msg' ->
            match msg' with
            | ShowWindowImage (WindowBitmap bitmap) ->
                sender <! (self, "ShowWindowImage")
                form.LogSetWindow( bitmap )
            | ShowBoardImage (BoardBitmap bitmap) ->
                sender <! (self, "ShowBoardImage")
                form.LogShowImage( bitmap )
        | :? FoundSixMsg as msg' ->
            match msg' with
            | FoundSix (CoordX x, CoordY y, None, pixels) ->
                let bitmap = pixels |> toBitmap
                form.LogShowImage( bitmap )
            | FoundSix (CoordX x, CoordY y, Some clue, pixels) ->
                let bitmap = pixels |> toBitmap
                form.LogShowImage( bitmap )
                form.LogClueOverlay( x, y, int clue)
        // board sides have been found
        | :? FoundSideMsg as msg' ->
            match msg' with
            | FoundTopSide (PixelY py) ->
                coord |> Option.map (fun c -> c <! (self, sprintf "FoundTopSide y:%i" py) ) |> ignore
                form.LogDrawHLine( py )
            | FoundBottomSide (PixelY py) ->
                coord |> Option.map (fun c -> c <! (self, sprintf "FoundBottomSide y:%i" py)) |> ignore
                form.LogDrawHLine( py )
            | FoundLeftSide (PixelX px) ->
                coord |> Option.map (fun c -> c <! (self, sprintf "FoundLeftSide x:%i" px)) |> ignore
                form.LogDrawVLine( px )
            | FoundRightSide (PixelX px) ->
                coord |> Option.map (fun c -> c <! (self, sprintf "FoundRightSide x:%i" px)) |> ignore
                form.LogDrawVLine( px )
        //| :? SidesFinalizedMsg  
        | :? FoundCellsMsg as msg' ->
            match msg' with
            | FoundCells (cols,rows) -> form.LogSizing(cols.Length, rows.Length)
        | :? FoundClueMsg as msg' ->
            coord |> Option.map (fun c -> c <! (self, sprintf "FoundClue %A" msg') ) |> ignore
            match msg' with
            | FoundClue (CoordX cx, CoordY cy, Some clue) ->
                form.LogClueOverlay( cx, cy, int clue )
            | FoundClue (CoordX cx, CoordY cy, None) ->
                form.LogBlankOverlay( cx, cy )
        //| :? CluesFinalizedMsg
        // Unhandled
        | _ ->
            sender <! (self, sprintf "Unhandled: %A" msg)
            self.Unhandled msg

        return! loop coord
    }

    loop None
