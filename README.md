# Fill-A-Pix

Solver in F# which reads a screencapture of a newly-created Conceptis Puzzles Fill-A-Pix puzzle.  Uses OCR to find
the game board and tiles, and to read values in from the tiles.  Then, shows a list of moves which will solve the
puzzle.  As of now, includes simple OCR which can compare one bitmap to another and return a difference value.
