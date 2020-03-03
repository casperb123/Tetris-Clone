using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

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

    public void Quit()
    {
        Application.Quit();
    }
}
