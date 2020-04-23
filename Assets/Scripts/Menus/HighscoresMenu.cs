using TMPro;
using UnityEngine;

public class HighscoresMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject backMenu;
    [SerializeField]
    private GameObject highscoresMenu;
    [SerializeField]
    private TMP_Text showButtonText;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip buttonClick;

    [Header("General Settings")]
    [SerializeField]
    private Highscores highscoresScript;

    private AudioSource audioSource;
    private bool showingMore;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Show()
    {
        if (buttonClick == null)
            audioSource.Play();
        else
            audioSource.PlayOneShot(buttonClick);

        if (showingMore)
        {
            highscoresScript.Top = 10;
            showButtonText.text = "Show More";
        }
        else
        {
            highscoresScript.Top = 100;
            showButtonText.text = "Show Less";
        }

        highscoresScript.GetHighScores();
        showingMore = !showingMore;
    }

    public void Back()
    {
        if (buttonClick == null)
            audioSource.Play();
        else
            audioSource.PlayOneShot(buttonClick);

        highscoresMenu.SetActive(false);
        backMenu.SetActive(true);
    }
}
