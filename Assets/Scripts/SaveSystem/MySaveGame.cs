using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class MySaveGame : SaveGame
{
    public DateTime TimeStamp { get; set; }
    public int Score { get; set; }
    public int Lines { get; set; }
    public List<SavedMino> Minos { get; private set; }

    public MySaveGame()
    {
        TimeStamp = DateTime.Now;
        Minos = new List<SavedMino>();
    }
}
