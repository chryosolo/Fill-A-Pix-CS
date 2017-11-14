module Fill_A_Pix.Messages

open GameTypes
open Akka.Actor
open Fill_A_Pix_CS

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
type StepMoveMsg = | StepMove
type StepMoveFromMsg = | StepMoveFrom of BoardState
type FoundMoveMsg = | FoundZeroClue of UpdateBoardState list
                    | FoundStartingClue of UpdateBoardState list
                    | FoundEnoughFilled of UpdateBoardState list
                    | FoundEnoughBlank of UpdateBoardState list
                    | FoundEndOfGame

type SetUiMsg = | SetUi of IActorRef
type SetUiFormMsg = | SetUiForm of FrmMain
type DrawCellsMsg = | DrawCells of Cell[]
