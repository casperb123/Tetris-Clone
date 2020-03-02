using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    private Game Game;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
    }

    public void PauseToggle()
    {
        if (Game.Instance.IsPaused)
        {
            pausePanel.SetActive(false);
            Game.Instance.IsPaused = false;
            audioSource.UnPause();
        }
        else
        {
            Game.Instance.IsPaused = true;
            pausePanel.SetActive(true);
            audioSource.Pause();
        }
    }

    public void Resume()
    {
        PauseToggle();
    }

    public void ExitToTitle()
    {
        Game.IsPaused = false;
        SceneManager.LoadScene("GameMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
