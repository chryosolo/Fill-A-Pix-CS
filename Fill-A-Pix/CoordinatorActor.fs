module Fill_A_Pix.CoordinatorActor

open Akka.FSharp
open Akka.Actor
open System.Drawing
open GameTypes
open Messages
open Utility

let actor (self:Actor<obj>) =
    let board = spawn self "board" BoardActor.actor

    // coordinator is currently working on recognizing
    let rec hasBitmap (ui:IActorRef) (bitmap:Bitmap) = actor {
        let! msg = self.Receive()
        match msg with
        | :? SidesFinalizedMsg as msg' ->
            match msg' with
            | SidesFinalized rect ->
                let clipped = bitmap |> clipBitmap rect
                ui <! ShowBoardImage (BoardBitmap clipped)
                return! hasBitmap ui clipped
        | :? StepMoveMsg -> board.Forward msg
        // forward to UI
        | :? FoundSideMsg
        | :? SidesFinalizedMsg
        | :? FoundCellsMsg
        | :? FoundClueMsg
        | :? CluesFinalizedMsg
        | :? SetBoardStateMsg
        | :? DrawCellsMsg -> ui.Forward msg
        | _ -> self.Unhandled msg
        return! hasBitmap ui bitmap
    }

    // coordinator can enter hasUi state once it gets an UI Actor
    let rec hasUi (ui:IActorRef) = actor {
        let! msg = self.Receive()
        match msg with
        | :? OpenBitmapFileMsg as msg' ->
            match msg' with
            | OpenBitmapFile (BitmapFile file) ->
                let bitmap = new Bitmap( file |> Image.FromFile )
                board <! SetWindow (bitmap |> toPixels)
                ui <! ShowWindowImage (bitmap |> clone |> WindowBitmap)
                return! hasBitmap ui bitmap
        | _ -> self.Unhandled msg

        return! hasUi ui
    }

    // coordinator starts without a UI Actor
    let rec noUi () = actor {
        let! msg = self.Receive()
        match msg with
        | :? SetUiMsg as msg' ->
            match msg' with
            | SetUi ui ->
                self.UnstashAll ()
                return! hasUi ui
        | _ -> self.Stash ()
    }

    noUi ()

