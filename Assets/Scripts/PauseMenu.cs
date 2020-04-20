using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject saveMenu;
    [SerializeField]
    private GameObject playMenu;
    [SerializeField]
    private GameObject saveBackButton;
    [SerializeField]
    private GameObject saveQuitButton;

    [Header("General Settings")]
    [SerializeField]
    private GameObject dialogTemplate;

    private AudioSource audioSource;
    private AudioSource audioSourceGameLoop;
    private bool isPaused;
    private SavedOptions options;
    private OptionsMenu optionsMenuScript;

    private void Start()
    {
        options = SaveSystem.GetOptions();
        audioSource = GetComponent<AudioSource>();
        optionsMenuScript = GetComponent<OptionsMenu>();
        audioSourceGameLoop = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseToggle();
    }

    private void OnApplicationFocus(bool focus)
    {
        options = SaveSystem.GetOptions();

        if (!focus && !optionsMenu.activeSelf && !saveMenu.activeSelf && !playMenu.activeSelf && options.AutoPauseOnFocusLose)
            Pause(false);
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
    private void Pause(bool playSound = true)
    {
        if (playSound)
            audioSource.Play();

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
        if (optionsMenuScript is null || optionsMenuScript.OptionsChanged)
            return;

        options = SaveSystem.GetOptions();
        audioSource.Play();
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

        if (Game.SaveGameChanged)
        {
            GameObject dialogObject = Instantiate(dialogTemplate, dialogTemplate.transform.parent);
            Dialog dialog = dialogObject.GetComponent<Dialog>();
            dialog.onResult += (Dialog.Result result) =>
            {
                audioSource.Play();

                if (result == Dialog.Result.Yes)
                {
                    Game.Instance.UpdateHighscores();
                    Game.SaveGame = null;
                    SceneManager.LoadScene("GameMenu");
                }
                else if (result == Dialog.Result.Save)
                {
                    SaveMenu.GoingToMenu = true;
                    saveQuitButton.SetActive(true);
                    saveBackButton.SetActive(false);

                    pauseMenu.SetActive(false);
                    saveMenu.SetActive(true);
                }

                Destroy(dialogObject);
            };

            dialog.Open(Dialog.Type.Save, "Are you sure that you want to go to the menu without saving?");
        }
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        audioSource.Play();

        if (Game.SaveGameChanged)
        {
            GameObject dialogObject = Instantiate(dialogTemplate, dialogTemplate.transform.parent);
            Dialog dialog = dialogObject.GetComponent<Dialog>();
            dialog.onResult += (Dialog.Result result) =>
            {
                audioSource.Play();

                if (result == Dialog.Result.Yes)
                    Application.Quit();
                else if (result == Dialog.Result.Save)
                {
                    SaveMenu.Quitting = true;
                    saveQuitButton.SetActive(true);
                    saveBackButton.SetActive(false);

                    pauseMenu.SetActive(false);
                    saveMenu.SetActive(true);
                }

                Destroy(dialogObject);
            };

            dialog.Open(Dialog.Type.Save, "Are you sure that you want to quit without saving?");
        }
        else
            Application.Quit();
    }
}
