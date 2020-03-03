using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject pauseMenu;

    private Game Game;
    private AudioSource audioSource;
    private AudioSource audioSourceGameLoop;
    private bool isPaused;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSourceGameLoop = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioSource>();
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
        audioSource.Play();

        if (isPaused)
        {
            pauseMenu.SetActive(false);
            audioSourceGameLoop.UnPause();
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            audioSourceGameLoop.Pause();
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        PauseToggle();
    }

    public void Menu()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
