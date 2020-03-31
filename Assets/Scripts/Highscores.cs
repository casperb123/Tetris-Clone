using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class Highscores : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField]
    private int top = 10;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject template;

    private void Start()
    {
        GetHighScores();
    }

    private void GetHighScores()
    {
        List<SavedHighscore> highscores = SaveSystem.GetHighscores();
        int amount = highscores.Count;

        if (top < amount)
            amount = top;

        for (int i = 0; i < amount; i++)
        {
            GameObject highscoreObject = Instantiate(template, transform);
            TMP_Text positionNameText = highscoreObject.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text scoreText = highscoreObject.transform.GetChild(1).GetComponent<TMP_Text>();

            positionNameText.text = $"{i + 1}. {highscores[i].Name}";
            scoreText.text = AddCommas(highscores[i].Score);
            highscoreObject.SetActive(true);
        }
    }

    private string AddCommas(int number)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:n0}", number);
    }
}
