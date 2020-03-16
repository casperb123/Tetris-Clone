using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int highScore;
    private AudioSource audioSource;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("highscore");
        audioSource = GetComponent<AudioSource>();

        highScoreText.text = highScore.ToString();
        scoreText.text = Game.Instance.CurrentScore.ToString();

        if (Options.Instance.SoundEffects)
            audioSource.Play();
    }

    public void Retry()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
