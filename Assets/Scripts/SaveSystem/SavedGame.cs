using System;
using System.Collections.Generic;

[Serializable]
public class SavedGame
{
    public DateTime? TimeStamp { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Lines { get; set; }
    public List<SavedMino> Minos { get; private set; }
    public SavedTetromino CurrentTetromino { get; set; }
    public SavedTetromino NextTetromino { get; set; }
    public SavedTetromino SavedTetromino { get; set; }

    public SavedGame()
    {
        TimeStamp = DateTime.Now;
        Minos = new List<SavedMino>();
    }
}
