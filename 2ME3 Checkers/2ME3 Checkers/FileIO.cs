﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace _2ME3_Checkers
{
    class FileIO:I_FileIOInterface
    {
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/CheckersGameSaves"; //designate a path for the folder

        public FileIO()
        {
        }

        public void save(Board board, Piece.PLAYER turn)
        {
            string saveText = "";
            switch (turn)
            {
                case (Piece.PLAYER.BLACK):
                    saveText += "BLACK\n";
                    break;
                case (Piece.PLAYER.WHITE):
                    saveText += "WHITE\n";
                    break;
            }
            int jAdjusted = 0;  //adding one to index j for board index

            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Save Unsuccessful " + e.Message);
            }

            for (int i = 0; i < board.getPieceArray().Length / 8; i++)
            {
                for (int j = 0; j < board.getPieceArray().Length / 8; j++)
                {
                    if (board.getPiece(i, j) != null)
                    {
                        //Saving Row
                        switch (i)
                        {
                            case (0):
                                saveText += "A";
                                break;
                            case (1):
                                saveText += "B";
                                break;
                            case (2):
                                saveText += "C";
                                break;
                            case (3):
                                saveText += "D";
                                break;
                            case (4):
                                saveText += "E";
                                break;
                            case (5):
                                saveText += "F";
                                break;
                            case (6):
                                saveText += "G";
                                break;
                            case (7):
                                saveText += "H";
                                break;
                        }

                        //Saving Column
                        jAdjusted = j + 1;
                        saveText += jAdjusted.ToString() + "=";

                        //Saving Colour
                        if (board.getPiece(i, j).getOwner() == Piece.PLAYER.WHITE)
                        {
                            saveText += "W";
                        }
                        else if (board.getPiece(i,j).getOwner() == Piece.PLAYER.BLACK)
                        {
                            saveText += "B";
                        }

                        //Saving Piece Type
                        if (board.getPiece(i, j).getType() == Piece.TYPESTATE.KING)
                        {
                            saveText += "K";
                        }

                        saveText += ",";
                    }
                }
            }
            saveText = saveText.TrimEnd(','); //Get rid of trailing comma 
            System.IO.File.WriteAllText(@path + "/SavedGame.txt", saveText); //write to file
            Console.WriteLine("Game Saved!");
        }

        public string[] load(Board board)
        {

            if ((File.Exists(@path + "/SavedGame.txt")))
            {
                String[] loaddata = System.IO.File.ReadAllText(@path + "/SavedGame.txt").Split();
                return loaddata;
            }
            else
            {
                throw new Exception("No Save Game!");
            }    
        }
    }
}