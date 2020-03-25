using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool NewHighScore;

    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI highScoreTextLabel;
    [SerializeField]
    private GameObject scoreContainer;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int highScore;
    private AudioSource audioSource;

    private void Start()
    {
#if !DEBUG
        if (Game.Instance is null)
        {
            SceneManager.LoadScene("GameMenu");
            return;
        }
#endif

        highScore = PlayerPrefs.GetInt("highscore");
        audioSource = GetComponent<AudioSource>();

        highScoreText.text = highScore.ToString();
        scoreText.text = Game.Instance.CurrentScore.ToString();

        if (NewHighScore)
        {
            scoreContainer.SetActive(false);
            highScoreTextLabel.text = "New Highscore!";
            highScoreTextLabel.color = Color.green;
            highScoreText.color = Color.green;
            NewHighScore = false;
        }

        if (OptionsMenu.Instance.SoundEffects)
            audioSource.Play();
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
