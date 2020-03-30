using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool NewHighscore;

    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI highScoreTextLabel;
    [SerializeField]
    private GameObject scoreContainer;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    private SavedHighscore highScore;
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

        highScore = SaveSystem.GetHighscore();
        audioSource = GetComponent<AudioSource>();
        options = SaveSystem.GetOptions();

        highScoreText.text = highScore.Score.ToString();
        scoreText.text = Game.Instance.CurrentScore.ToString();

        if (NewHighscore)
        {
            scoreContainer.SetActive(false);
            highScoreTextLabel.text = "New Highscore!";
            highScoreTextLabel.color = Color.green;
            highScoreText.color = Color.green;
            NewHighscore = false;
        }

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

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick);
        Application.Quit();
    }
}
