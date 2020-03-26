using System;
using System.Collections.Generic;

[Serializable]
public class MySaveGame : SaveGame
{
    public DateTime? TimeStamp { get; set; }
    public int Score { get; set; }
    public int Lines { get; set; }
    public List<SavedMino> Minos { get; private set; }
    public SavedTetromino CurrentTetromino { get; set; }
    public SavedTetromino NextTetromino { get; set; }
    public SavedTetromino SavedTetromino { get; set; }

    public MySaveGame()
    {
        TimeStamp = DateTime.Now;
        Minos = new List<SavedMino>();
    }
}
