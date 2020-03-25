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
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
    }

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

    public void LoadMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    public void OptionsMenu()
    {
        audioSource.PlayOneShot(buttonClick);
        gameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick);
        Application.Quit();
    }

    public void ChangedLevel(float level)
    {
        audioSource.PlayOneShot(sliderClick);
        startingLevel = (int)level;
        levelText.text = level.ToString();
    }
}
