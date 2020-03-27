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
    private SavedOptions options;

    private void Start()
    {
        options = SaveSystem.GetOptions();
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

    /// <summary>
    /// Pauses the game
    /// </summary>
    private void Pause()
    {
        pauseMenu.SetActive(true);
        audioSourceGameLoop.Stop();
        isPaused = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    private void UnPause()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        saveMenu.SetActive(false);
        if (options != null && options.BackgroundMusic)
            audioSourceGameLoop.Play();
        isPaused = false;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Toggles the pause state
    /// </summary>
    public void PauseToggle()
    {
        audioSource.Play();

        if (isPaused)
            UnPause();
        else
            Pause();
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        PauseToggle();
    }

    /// <summary>
    /// Opens the options menu
    /// </summary>
    public void Options()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Opens the save menu
    /// </summary>
    public void Save()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
        saveMenu.SetActive(true);
    }

    /// <summary>
    /// Goes to the game menu
    /// </summary>
    public void Menu()
    {
        audioSource.Play();
        SceneManager.LoadScene("GameMenu");
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        audioSource.Play();
        Application.Quit();
    }
}
