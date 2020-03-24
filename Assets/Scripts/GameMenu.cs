using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static MySaveGame SaveGame;

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
        SaveGame = null;
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
        gameMenu.SetActive(false);
        loadMenu.SetActive(true);
        audioSource.PlayOneShot(buttonClick);
    }

    public void OptionsMenu()
    {
        gameMenu.SetActive(false);
        optionsMenu.SetActive(true);
        audioSource.PlayOneShot(buttonClick);
    }

    public void Quit()
    {
        audioSource.PlayOneShot(buttonClick);
        Application.Quit();
    }

    public void ChangedLevel(float level)
    {
        startingLevel = (int)level;
        levelText.text = level.ToString();
        audioSource.PlayOneShot(sliderClick);
    }
}
