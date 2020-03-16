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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSourceGameLoop = Game.Instance.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            Pause();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Pause();
        else
            UnPause();
    }

    private void Pause()
    {
        pausePanel.SetActive(true);
        audioSourceGameLoop.Stop();
        isPaused = true;
        Time.timeScale = 0;
    }

    private void UnPause()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        if (Options.Instance.BackgroundMusic)
            audioSourceGameLoop.Play();
        isPaused = false;
        Time.timeScale = 1;
    }

    public void PauseToggle()
    {
        audioSource.Play();

        if (isPaused)
            UnPause();
        else
            Pause();
    }

    public void Resume()
    {
        PauseToggle();
    }

    public void OptionsMenu()
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
