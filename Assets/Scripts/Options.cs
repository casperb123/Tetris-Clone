using System;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    private bool backgroundMusic;
    private bool soundEffects;
    private bool shakingEffect;
    private AudioSource audioSource;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject optionsPanel;
    [SerializeField]
    private Toggle backgroundMusicToggle;
    [SerializeField]
    private Toggle soundEffectsToggle;
    [SerializeField]
    private Toggle shakingEffectToggle;

    public bool BackgroundMusic
    {
        get => backgroundMusic;
    }

    public bool SoundEffects
    {
        get => soundEffects;
    }

    public bool ShakingEffect
    {
        get => shakingEffect;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        backgroundMusic = Convert.ToBoolean(PlayerPrefs.GetInt("backgroundMusic"));
        soundEffects = Convert.ToBoolean(PlayerPrefs.GetInt("soundEffects"));
        shakingEffect = Convert.ToBoolean(PlayerPrefs.GetInt("shakingEffect"));

        backgroundMusicToggle.isOn = backgroundMusic;
        soundEffectsToggle.isOn = soundEffects;
        shakingEffectToggle.isOn = shakingEffect;
    }

    public void ToggleBackgroundMusic(bool value)
    {
        backgroundMusic = value;
        PlayerPrefs.SetInt("backgroundMusic", Convert.ToInt32(backgroundMusic));
        audioSource.Play();
    }

    public void ToggleSoundEffects(bool value)
    {
        soundEffects = value;
        PlayerPrefs.SetInt("soundEffects", Convert.ToInt32(soundEffects));
        audioSource.Play();
    }

    public void ToggleShakingEffect(bool value)
    {
        shakingEffect = value;
        PlayerPrefs.SetInt("shakingEffect", Convert.ToInt32(shakingEffect));
        audioSource.Play();
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
        audioSource.Play();
    }
}
