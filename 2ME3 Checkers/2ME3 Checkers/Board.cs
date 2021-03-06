﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2ME3_Checkers
{
    public class Board : I_BoardInterface
    {
        //variables
        private Piece[,] pieceArray = new Piece[8,8];
        private int numWhitePieces = 0;
        private int numBlackPieces = 0;
        private int numPieces = 0; // Number of pieces of a specified colour
        private Piece jumpingPiece = null; // This variable saves the Piece that is currently jumping
        private bool[] jumpAvailable = new bool [2]; // Whether a jump can be made and by which player. index 0 is white, index 1 is black. The array instances are boolean
        //Constructors

        /// <summary>
        /// Default setup of the board
        /// </summary>
        public Board()
        {
            this.clear(); // clear the board first

            // default setup
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if ((col % 2 == 0 && (row == 0 || row == 2)) || (col % 2 != 0 && row == 1)) // bottom PLAYER's area
                        placePiece(col, row, new Piece(Piece.TYPESTATE.NORMAL, Piece.PLAYER.WHITE));
                    else if ((col % 2 != 0 && (row == 5 || row == 7)) || (col % 2 == 0 && row == 6)) // top PLAYER's area
                        placePiece(col, row, new Piece(Piece.TYPESTATE.NORMAL, Piece.PLAYER.BLACK));
                    else
                    {
                        // place nothing
                    }
                        
                }
            }
        }

        /// <summary>
        /// Custom setup of the board
        /// <param name="input"> the input string which will be interpreted as the piece locations </param>
        /// Throws an exceptions on all malformed inputs. This whole constructor will be within a try catch statement in the main file. 
        /// If the constructor has an exception, the main file will know the input is invalid
        /// </summary>
        public void setUpBoard (String input)
        {
            // sample input string: "A1=W,C1=W,E1=W,G1=WK,A7=B,B8=B"
            // the goal is to parse this string and place the pieces on the board
            numBlackPieces = 0;
            numWhitePieces = 0;
            clear(); // we assume the user will setup the board all at once in one line
                     // we can later give the option to allow the user to set up the board one piece at a time.

            string[] splitCommas = input.Split(','); // splits "A1=W,C1=W" on the comma
            string[] splitEquals;
            int coordCol;
            int coordRow;

            Piece.PLAYER player;
            Piece.TYPESTATE type;
            for (int i = 0; i < splitCommas.Length; i++)
            {
                splitEquals = splitCommas[i].Split('='); // split "A1=W" on the equals sign
                if (splitEquals[0].Length != 2) throw new Exception("Board input format error"); // if left side is not "A1", but "A12" or "AA1" then error
                coordRow = Convert.ToInt16(splitEquals[0].Substring(1, 1)); // convert the row number an int
                // internally 0-7 instead of 1-8 so we subtract 1
                coordRow -= 1;

                switch (splitEquals[0].Substring(0,1).ToUpper()) { // associate the column names with its index
                    case ("A"):
                        coordCol = 0;
                        break;
                    case ("B"):
                        coordCol = 1;
                        break;
                    case ("C"):
                        coordCol = 2;
                        break;
                    case ("D"):
                        coordCol = 3;
                        break;
                    case ("E"):
                        coordCol = 4;
                        break;
                    case ("F"):
                        coordCol = 5;
                        break;
                    case ("G"):
                        coordCol = 6;
                        break;
                    case ("H"):
                        coordCol = 7;
                        break;
                    default:
                        //if the input isn't recognized, then throw an exception
                        throw new Exception("Input Error");
                }

                switch (splitEquals[1].ToUpper()) // the right side of the equal sign in A1=W
                {
                    case ("B"):
                        player = Piece.PLAYER.BLACK;
                        type = Piece.TYPESTATE.NORMAL;
                        numBlackPieces++;
                        break;
                    case ("W"):
                        player = Piece.PLAYER.WHITE;
                        type = Piece.TYPESTATE.NORMAL;
                        numWhitePieces++;
                        break;
                    case ("BK"):
                        player = Piece.PLAYER.BLACK;
                        type = Piece.TYPESTATE.KING;
                        numBlackPieces++;
                        break;
                    case ("WK"):
                        player = Piece.PLAYER.WHITE;
                        type = Piece.TYPESTATE.KING;
                        numWhitePieces++;
                        break;
                    default:
                        //if the input isn't recognized, then throw an exception
                        throw new Exception("Input Error");
                }
 
                placePiece(coordCol, coordRow, new Piece(type, player)); // place the piece with the parsed information
            }

        }

        /// <summary>
        /// This method is used to determine if a piece exists on a spot (returns null for no).
        /// If the piece does exist, we pass it along to the caller so it can do specific Piece methods such as getType() or getOwner().
        /// </summary>
        public Piece getPiece(int column, int row)
        {
            try { return pieceArray[column, row]; }
            catch { throw new Exception("Error: Trying to fetch piece outside of array"); }
        }

        public Piece getPiece(Vector2 location)
        {
            try { return pieceArray[(int) location.X, (int) location.Y]; }
            catch { throw new Exception("Error: Trying to fetch piece outside of array"); }
        }

        public Piece[,] getPieceArray()
        {
            return pieceArray;
        }
        // the piece that is currently jumping
        public Piece getJumpingPiece()
        {
            return jumpingPiece;
        }

        public void setJumpingPiece(Piece newJumpingPiece)
        {
            jumpingPiece = newJumpingPiece;
        }
        //Sets whether a jump is available for the indicated player
        public void setJumpAvailable(Piece.PLAYER colour, bool jumpAvailability)
        {
            if(colour == Piece.PLAYER.WHITE)
                this.jumpAvailable[0] = jumpAvailability;
            else if (colour == Piece.PLAYER.BLACK)
                this.jumpAvailable[1] = jumpAvailability;
        }
        //Gets whether a jump is available for the indicated player
        public bool getJumpAvailable(Piece.PLAYER colour)
        {
            if (colour == Piece.PLAYER.WHITE)
                return this.jumpAvailable[0];
            else if (colour == Piece.PLAYER.BLACK)
                return jumpAvailable[1];
            else
                throw new Exception("Trying to return if jumps are available for an unknown player");
        }
        //Gets whether a jump is available for both players
        public bool[] getJumpAvailable()
        {
            return this.jumpAvailable;
        }
        //returns the number of specified colour pieces on the board
        public int getNumPieces(Piece.PLAYER colour)
        {
            numPieces = 0; //initialize that amount to 0
            int t_numOtherPieces = 0; //temp var for the number of pieces not specified
            // the number of other pieces is checked because if both are 0, the board is empty

            //add one for each instance found in the array
            for (int i = 0; i < Math.Sqrt(pieceArray.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(pieceArray.Length); j++)
                {
                    if (pieceArray[i, j] != null)
                    {
                        try
                        {
                            if (pieceArray[i, j].getOwner() == colour)
                                numPieces++;
                            if (pieceArray[i, j].getOwner() != colour)
                                t_numOtherPieces++;
                        }
                        catch { };
                    }
                }
            }
            if (!(numPieces == 0 && t_numOtherPieces == 0))
                return numPieces;
            else
                return -99;
        }

        /// <summary>
        /// Checks if a piece placement is legal, if so, it will place the piece there.
        /// </summary>
        public void placePiece(int column, int row, Piece piece) 
        {
            // Too many pieces check
            // Another safer option is to recount the pieceArray every time we add a piece instead of having a running count.
            if (numBlackPieces > 12 || numWhitePieces > 12)
            {
                Console.Write("You can only have up to 12 of one kind of piece and ");
                if (numWhitePieces > numBlackPieces)
                    Console.WriteLine("you had " + numWhitePieces + " white pieces");
                else
                    Console.WriteLine("you had " + numBlackPieces + " black pieces");
                throw new Exception();
            }

            // Invalid placement check
            if (column % 2 != row % 2)
            {
                Console.WriteLine("Invalid placement. Only place on solid board squares");
                throw new Exception();
            }
            // The tests all passed. Add the piece to the array
            pieceArray[column, row] = piece;
        }

        /// <summary>
        /// Checks if the movement of a piece is legal, if so, then we move the piece to that location.
        /// </summary>
        public void movePiece(int fromCol, int fromRow, int toCol, int toRow)
        {
            // check if movement is legal is done before this function is called

            // put piece into new location
            this.pieceArray[toCol, toRow] = this.pieceArray[fromCol, fromRow];
            // remove piece from previous location
            this.pieceArray[fromCol, fromRow] = null;
        }
        public void movePiece(Vector2 originalLocation, Vector2 newLocation)
        {
            // check if movement is legal is done before this function is called

            // put piece into new location
            this.pieceArray[(int)newLocation.X, (int)newLocation.Y] = this.pieceArray[(int)originalLocation.X, (int)originalLocation.Y];
            // remove piece from previous location
            this.pieceArray[(int)originalLocation.X, (int)originalLocation.Y] = null;
        }

        public void removePiece(int column, int row)
        {
            try { pieceArray[column, row] = null; }
            catch { throw new Exception("Error: Trying to remove piece outside of array"); }
        }

        /// <summary>
        /// Clears the board
        /// </summary>
        public void clear()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    pieceArray[col, row] = null;
                }
            }
        }
    }
}
