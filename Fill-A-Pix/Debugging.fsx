#load "Game.fs"
#load "OCR.fs"
#r "System.Drawing.dll"

open Game
open OCR
open System.Drawing

let window = new Bitmap( Image.FromFile( "C:\\git\\Sandbox\\Fill-A-Pix\\2017-03-03.png" ) )
let boardBitmap = findBoard window
let clues = findClues boardBitmap
let board = toBoard clues
let initialState = (board,[BeginningOfGame])

