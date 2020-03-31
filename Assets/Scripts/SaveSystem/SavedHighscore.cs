using System;

[Serializable]
public class SavedHighscore
{
    public int Score { get; set; }
    public string Name { get; set; }
    public DateTime? Date { get; set; }

    public SavedHighscore(string name, int score)
    {
        Date = DateTime.Now;
        Name = name;
        Score = score;
    }
}
