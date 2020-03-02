using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text highScoreText;

    private void Start()
    {
        if (highScoreText != null)
            highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
    }

    public void PlayGame()
    {
        if (Game.StartingLevel == 0)
            Game.StartingAtLevelZero = true;
        else
            Game.StartingAtLevelZero = false;

        SceneManager.LoadScene("Level");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangedLevel(float level)
    {
        Game.StartingLevel = (int)level;
        levelText.text = level.ToString();
    }
}
