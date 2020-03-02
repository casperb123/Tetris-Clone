using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Text highScoreText;
    [SerializeField]
    private Text scoreText;

    private int highScore;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highscore");

        highScoreText.text = highScore.ToString();
        scoreText.text = Game.Instance.CurrentScore.ToString();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
