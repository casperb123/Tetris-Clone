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
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject playAgainPanel;
    [SerializeField]
    private GameObject pausePanel;

    private AudioSource audioSource;

    private void Start()
    {
        TryGetComponent(out audioSource);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void PlayGame()
    {
        if (Game.StartingLevel == 0)
            Game.StartingAtLevelZero = true;
        else
            Game.StartingAtLevelZero = false;

        Game.CurrentScore = 0;
        SceneManager.LoadScene("Level");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitToTitleScreen()
    {
        Game.IsPaused = false;
        SceneManager.LoadScene("GameMenu");
    }

    public void Cancel()
    {
        playAgainPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        gameOverPanel.SetActive(false);
        playAgainPanel.SetActive(true);
    }

    public void Pause()
    {
        if (Game.IsPaused)
        {
            pausePanel.SetActive(false);
            Game.IsPaused = false;

            if (audioSource != null)
                audioSource.UnPause();
        }
        else
        {
            if (!playAgainPanel.activeSelf && !gameOverPanel.activeSelf)
            {
                Game.IsPaused = true;
                pausePanel.SetActive(true);
                audioSource.Pause();
            }
        }
    }

    public void ChangedLevel(float level)
    {
        Game.StartingLevel = (int)level;
        levelText.text = level.ToString();
    }
}
