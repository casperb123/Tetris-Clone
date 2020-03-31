using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool NewHighscore;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject highscoresMenu;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    private AudioSource audioSource;
    private SavedOptions options;

    private void Start()
    {
#if !DEBUG
        if (Game.Instance is null)
        {
            SceneManager.LoadScene("GameMenu");
            return;
        }
#endif

        audioSource = GetComponent<AudioSource>();
        options = SaveSystem.GetOptions();
        if (options.SoundEffects)
            audioSource.Play();
    }

    /// <summary>
    /// Goes back to the game menu
    /// </summary>
    public void Menu()
    {
        audioSource.PlayOneShot(buttonClick);
        SceneManager.LoadScene("GameMenu");
    }

    public void HighscoresMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameOverMenu.SetActive(false);
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
}
