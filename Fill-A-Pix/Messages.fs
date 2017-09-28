module Fill_A_Pix.Messages

open System.Drawing
open System.IO
open GameTypes
open Akka.FSharp
open Akka.Actor
open System.Windows.Forms
open Fill_A_Pix_UI

// Select a File
type OpenBitmapFileMsg = | OpenBitmapFile of BitmapFile
type ShowImageMsg = | ShowWindowImage of WindowBitmap
                    | ShowBoardImage of BoardBitmap
type SetWindowMsg = | SetWindow of Pixels

// Detect board
type DetectSidesMsg = | DetectSides of Pixels
type FoundSideMsg = | FoundTopSide of PixelY
                    | FoundBottomSide of PixelY
                    | FoundLeftSide of PixelX
                    | FoundRightSide of PixelX
type SidesFinalizedMsg = | SidesFinalized of PixelRect
type DetectCellsMsg = | DetectCells of Pixels
type FoundCellsMsg = | FoundCells of (CoordX*PixelX*PixelX)[]*(CoordY*PixelY*PixelY)[]
type DetectClueMsg = | DetectClue of CoordX*CoordY*Pixels
type FoundClueMsg = | FoundClue of CoordX*CoordY*(Clue option)
type CluesFinalizedMsg = | CluesFinalized of Cell[,]
type SetBoardStateMsg = | SetBoardState of BoardState

// play game
type StepMoveMsg = | StepMove // to coordinator
type FoundMoveMsg = | BeginningOfGame
                    | NothingFound
                    | StartingClue of Cell*BoardState
                    | EnoughFilled of Cell*BoardState
                    | EnoughBlank of Cell*BoardState
type UpdateBoardStateMsg = | UpdateCellState of Cell*CellState
                           | UpdateClueState of Cell*ClueState

type SetUiMsg = | SetUi of IActorRef
type SetUiFormMsg = | SetUiForm of FrmMain

type FoundSixMsg = | FoundSix of CoordX * CoordY * (Clue option)*Pixels

type Log = Actor<obj>*string

let logHandler (msg:Log) =
    let (actor,action) = msg
    let entry =
        sprintf "%s: %s"
            (actor.Self.Path.ToStringWithoutAddress())
            action
    File.AppendAllLines( "C:\\log.txt", [|entry|] )
