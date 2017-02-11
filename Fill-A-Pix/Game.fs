module Game

type CellState =
    | Unknown
    | Filled
    | Empty

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

type Cell = {X:int; Y:int; State:CellState; Clue:Clue}

type Board = {Rows:int; Cols:int; Cells:Cell[]}