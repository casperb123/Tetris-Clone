using System;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public static Options Instance;

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

    private AudioSource audioSource;

    public bool BackgroundMusic { get; private set; }

    public bool SoundEffects { get; private set; }

    public bool ShakingEffect { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        BackgroundMusic = Convert.ToBoolean(PlayerPrefs.GetInt("backgroundMusic"));
        SoundEffects = Convert.ToBoolean(PlayerPrefs.GetInt("soundEffects"));
        ShakingEffect = Convert.ToBoolean(PlayerPrefs.GetInt("shakingEffect"));

        backgroundMusicToggle.isOn = BackgroundMusic;
        soundEffectsToggle.isOn = SoundEffects;
        shakingEffectToggle.isOn = ShakingEffect;
    }

    public void ToggleBackgroundMusic(bool value)
    {
        BackgroundMusic = value;
        PlayerPrefs.SetInt("backgroundMusic", Convert.ToInt32(BackgroundMusic));
        audioSource.Play();
    }

    public void ToggleSoundEffects(bool value)
    {
        SoundEffects = value;
        PlayerPrefs.SetInt("soundEffects", Convert.ToInt32(SoundEffects));
        audioSource.Play();
    }

    public void ToggleShakingEffect(bool value)
    {
        ShakingEffect = value;
        PlayerPrefs.SetInt("shakingEffect", Convert.ToInt32(ShakingEffect));
        audioSource.Play();
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
        audioSource.Play();
    }
}
