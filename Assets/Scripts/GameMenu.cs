using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI highScoreText;

    [Header("Sound Settings")]
    [SerializeField]
    private AudioClip sliderClick;
    [SerializeField]
    private AudioClip buttonClick;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
    }

    public void Play()
    {
        audioSource.PlayOneShot(buttonClick);

        if (Game.StartingLevel == 0)
            Game.StartingAtLevelZero = true;
        else
            Game.StartingAtLevelZero = false;

        SceneManager.LoadScene("Level");
    }

    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick);
        Application.Quit();
    }

    public void ChangedLevel(float level)
    {
        Game.StartingLevel = (int)level;
        levelText.text = level.ToString();
        audioSource.PlayOneShot(sliderClick);
    }
}
