using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class Highscores : MonoBehaviour
{
    [Header("General Settings")]
    public int Top = 10;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject template;
    [SerializeField]
    private GameObject showButton;

    [HideInInspector]
    public List<SavedHighscore> HighscoresList;

    private void Start()
    {
        GetHighScores();
    }

    public void GetHighScores()
    {
        foreach (Transform child in transform)
        {
            if (child != template.transform)
                Destroy(child.gameObject);
        }

        HighscoresList = SaveSystem.GetHighscores();
        int amount = HighscoresList.Count;

        if (amount > Top)
            amount = Top;

        for (int i = 0; i < amount; i++)
        {
            GameObject highscoreObject = Instantiate(template, transform);
            TMP_Text positionNameText = highscoreObject.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text scoreText = highscoreObject.transform.GetChild(1).GetComponent<TMP_Text>();

            positionNameText.text = $"{i + 1}. {HighscoresList[i].Name}";
            scoreText.text = AddCommas(HighscoresList[i].Score);
            highscoreObject.SetActive(true);
        }

        if (showButton != null && HighscoresList.Count > 10)
            showButton.SetActive(true);
    }

    private string AddCommas(int number)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:n0}", number);
    }
}
