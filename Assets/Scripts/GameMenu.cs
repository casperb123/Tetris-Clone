using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private GameObject gameMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject loadMenu;
    [SerializeField]
    private GameObject highscoresMenu;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip sliderClick;
    [SerializeField]
    private AudioClip buttonClick;

    private AudioSource audioSource;
    private int startingLevel;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Starts the tetris game
    /// </summary>
    public void Play()
    {
        audioSource.PlayOneShot(buttonClick);
        Game.StartingLevel = startingLevel;

        if (Game.StartingLevel == 0)
            Game.StartingAtLevelZero = true;
        else
            Game.StartingAtLevelZero = false;

        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Opens the load menu
    /// </summary>
    public void LoadMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    /// <summary>
    /// Opens the options menu
    /// </summary>
    public void OptionsMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void HighscoresMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameMenu.SetActive(false);
        highscoresMenu.SetActive(true);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick);
        Application.Quit();
    }

    /// <summary>
    /// Changes the starting level
    /// </summary>
    /// <param name="level">The level to change to</param>
    public void ChangedLevel(float level)
    {
        audioSource.PlayOneShot(sliderClick);
        startingLevel = (int)level;
        levelText.text = level.ToString();
    }
}
