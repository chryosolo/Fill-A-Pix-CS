module Fill_A_Pix.CoordinatorActor

open Akka.FSharp
open Akka.Actor
open System.Drawing
open GameTypes
open Messages
open Utility

let actor (self:Actor<obj>) =
    let board = spawn self "board" BoardActor.actor
    let game = spawn self "game" GameActor.actor
    let logger = spawn self "log'" (actorOf logHandler)

    // coordinator is currently working on a board
    let rec hasBitmap (ui:IActorRef) (bitmap:Bitmap) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log as entry -> logger.Forward entry
        | :? SidesFinalizedMsg as msg' ->
            match msg' with
            | SidesFinalized rect ->
                let clipped = bitmap |> clipBitmap rect
                ui <! ShowBoardImage (BoardBitmap clipped)
                return! hasBitmap ui clipped
        // forward to game
        | :? StepMoveMsg -> game.Forward msg
        | :? FoundSixMsg -> ui.Forward msg
        // forward to UI
        | :? FoundSideMsg
        | :? SidesFinalizedMsg
        | :? FoundCellsMsg
        | :? FoundClueMsg
        | :? CluesFinalizedMsg
        | :? FoundMoveMsg ->
            logger <! (self, sprintf "hasBitmap.[UI Message] %A" msg)
            ui.Forward msg
        | _ ->
            logger <! (self, sprintf "hasBitmap.Unhandled: %A" msg)
            self.Unhandled msg
        return! hasBitmap ui bitmap
    }

    // coordinator can enter hasUi state once it gets an UI Actor
    let rec hasUi (ui:IActorRef) = actor {
        let! msg = self.Receive()
        match msg with
        | :? Log as entry -> logger.Forward entry
        | :? OpenBitmapFileMsg as msg' ->
            match msg' with
            | OpenBitmapFile (BitmapFile file) ->
                logger <! (self, sprintf "hasUi.OpenBitmapFile of %s" file)
                let bitmap = new Bitmap( file |> Image.FromFile )
                board <! SetWindow (bitmap |> toPixels)
                ui <! ShowWindowImage (bitmap |> clone |> WindowBitmap)
                return! hasBitmap ui bitmap
        | _ ->
            logger <! (self, sprintf "hasUi.Unhandled: %A" msg)
            self.Unhandled msg

        return! hasUi ui
    }

    // coordinator starts without a UI Actor
    let rec noUi () = actor {
        let! msg = self.Receive()
        match msg with
        | :? SetUiMsg as msg' ->
            match msg' with
            | SetUi ui ->
                logger <! (self, sprintf "noUi.SetUi of %A" ui.Path)
                self.UnstashAll ()
                return! hasUi ui
        | _ ->
            logger <! (self, sprintf "noUi.Stashing: %A" msg)
            self.Stash ()
    }

    noUi ()

