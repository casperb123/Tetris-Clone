using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text highScoreText;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip sliderClick;
    [SerializeField]
    private AudioClip buttonClick;

    private Game game;
    private AudioSource audioSource;

    private void Start()
    {
        game = Game.Instance;
        audioSource = GetComponent<AudioSource>();

        if (highScoreText != null)
            highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
    }

    public void PlayGame()
    {
        audioSource.PlayOneShot(buttonClick);

        if (Game.StartingLevel == 0)
            Game.StartingAtLevelZero = true;
        else
            Game.StartingAtLevelZero = false;

        SceneManager.LoadScene("Level");
    }

    public void ExitGame()
    {
        audioSource.PlayOneShot(buttonClick);
        while (audioSource.isPlaying){}
        Application.Quit();
    }

    public void ChangedLevel(float level)
    {
        Game.StartingLevel = (int)level;
        levelText.text = level.ToString();
        audioSource.PlayOneShot(sliderClick);
    }
}
