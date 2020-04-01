using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static int Score;
    public static bool NewHighscore;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject highscoresMenu;
    [SerializeField]
    private TMP_Text scoreTextLabel;
    [SerializeField]
    private TMP_Text scoreText;

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

        if (NewHighscore)
        {
            scoreTextLabel.text = "New Highscore!";
            scoreTextLabel.color = Color.green;
            scoreText.color = Color.green;
        }

        audioSource = GetComponent<AudioSource>();
        options = SaveSystem.GetOptions();
        scoreText.text = AddCommas(Score);

        if (options.SoundEffects)
            audioSource.Play();
    }

    private string AddCommas(int number)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:n0}", number);
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
