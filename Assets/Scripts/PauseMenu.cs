using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject saveMenu;

    private AudioSource audioSource;
    private AudioSource audioSourceGameLoop;
    private bool isPaused;
    private Game game;

    private void Start()
    {
        game = Game.Instance;
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
        if (!focus && !optionsMenu.activeSelf && !saveMenu.activeSelf)
            Pause();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Pause();
        else if (!pause && !optionsMenu.activeSelf && !saveMenu.activeSelf)
            UnPause();
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        audioSourceGameLoop.Stop();
        isPaused = true;
        Time.timeScale = 0;
    }

    private void UnPause()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        if (global::OptionsMenu.Instance.BackgroundMusic)
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
        audioSource.Play();
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void SaveMenu()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
        saveMenu.SetActive(true);
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
