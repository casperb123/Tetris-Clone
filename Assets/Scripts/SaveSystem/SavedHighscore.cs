using System;

[Serializable]
public class SavedHighscore
{
    public int Score { get; set; }
    public DateTime? Date { get; set; }

    public SavedHighscore()
    {
        Date = DateTime.Now;
    }
}
