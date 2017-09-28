namespace Fill_A_Pix_Types

module Messages =

    open GameTypes
    open Akka.FSharp
    open Akka.Actor

    // Select a File
    type OpenBitmapFileMsg = | OpenBitmapFile of BitmapFile
    type ShowImageMsg = | ShowWindowImage of WindowBitmap
                        | ShowBoardImage of BoardBitmap
    type DetectBoardMsg = | DetectBoard of WindowBitmap

    // Detect board
    type DetectSidesMsg = | DetectSides of WindowBitmap
    type FoundSideMsg = | FoundTopSide of PixelY
                        | FoundBottomSide of PixelY
                        | FoundLeftSide of PixelX
                        | FoundRightSide of PixelX
    type SidesFinalizedMsg = | SidesFinalized of PixelRect
    type DetectCellsMsg = | DetectCells of BoardBitmap
    type FoundCellsMsg = | FoundCells of (CoordX*PixelX*PixelX)[]*(CoordY*PixelY*PixelY)[]
    type DetectClueMsg = | DetectClue of CoordX*CoordY*CellBitmap
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
