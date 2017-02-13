module Game

type CellState =
    | Unknown
    | Filled
    | Empty

type ClueState =
    | Active
    | Used

type GivenClue =
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5
    | Six = 6
    | Seven = 7
    | Eight = 8
    | Nine = 9

type Clue = 
    | Blank
    | Given of GivenClue

type Cell = {State:CellState; Clue:Clue; ClueState:ClueState}

type Board = {Rows:int; Cols:int; Cells:Cell[,]}