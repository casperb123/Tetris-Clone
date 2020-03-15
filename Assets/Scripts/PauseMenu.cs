using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject optionsPanel;

    private AudioSource audioSource;
    private AudioSource audioSourceGameLoop;
    private bool isPaused;
    private Options options;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSourceGameLoop = Game.Instance.GetComponent<AudioSource>();
        options = GetComponent<Options>();
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
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            if (options.BackgroundMusic)
                audioSourceGameLoop.Play();
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            pausePanel.SetActive(true);
            audioSourceGameLoop.Stop();
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        PauseToggle();
    }

    public void Options()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
        audioSource.Play();
    }

    public void Menu()
    {
        audioSource.Play();
        SceneManager.LoadScene("GameMenu");
    }

    public void Quit()
    {
        audioSource.Play();
        Application.Quit();
    }
}
