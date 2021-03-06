﻿using System;
using System.Collections.Generic;

[Serializable]
public class SavedGame
{
    private DateTime timeStamp;

    public DateTime TimeStamp
    {
        get => timeStamp;
        set
        {
            if (value < DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(TimeStamp), "The timestamp can't be lower than the current date and time");

            timeStamp = value;
        }
    }

    public int Slot { get; set; }
    public DateTime? LastLoaded { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Lines { get; set; }
    public List<SavedMino> Minos { get; private set; }
    public SavedTetromino CurrentTetromino { get; set; }
    public SavedTetromino NextTetromino { get; set; }
    public SavedTetromino SavedTetromino { get; set; }

    public SavedGame()
    {
        timeStamp = DateTime.Now;
        Minos = new List<SavedMino>();
    }

    public SavedGame(int slot, DateTime lastLoaded, string name, int score, int lines, SavedTetromino currentTetromino, SavedTetromino nextTetromino, SavedTetromino savedTetromino) : this()
    {
        Slot = slot;
        LastLoaded = lastLoaded;
        Name = name;
        Score = score;
        Lines = lines;
        CurrentTetromino = currentTetromino;
        NextTetromino = nextTetromino;
        SavedTetromino = savedTetromino;
    }
}
