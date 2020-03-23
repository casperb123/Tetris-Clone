using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

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
    [SerializeField]
    private Toggle fullscreenToggle;
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private TMP_Dropdown refreshRateDropdown;

    private AudioSource audioSource;
    private List<Resolution> resolutions;
    private List<int> refreshRates;

    public bool BackgroundMusic { get; private set; }

    public bool SoundEffects { get; private set; }

    public bool ShakingEffect { get; private set; }

    public bool Fullscreen { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int RefreshRate { get; private set; }

    private void Awake()
    {
        if (Instance is null)
            Instance = this;

        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        resolutions = Screen.resolutions.Select(x => new Resolution { width = x.width, height = x.height }).Distinct().Reverse().ToList();
        refreshRates = Screen.resolutions.Select(x => x.refreshRate).Distinct().Reverse().ToList();
        resolutions.ForEach(x => resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{x.width} x {x.height}")));
        refreshRates.ForEach(x => refreshRateDropdown.options.Add(new TMP_Dropdown.OptionData($"{x} hz")));

        BackgroundMusic = Convert.ToBoolean(PlayerPrefs.GetInt("backgroundMusic", 1));
        SoundEffects = Convert.ToBoolean(PlayerPrefs.GetInt("soundEffects", 1));
        ShakingEffect = Convert.ToBoolean(PlayerPrefs.GetInt("shakingEffect", 1));
        Fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen", 1));
        Width = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        Height = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        RefreshRate = PlayerPrefs.GetInt("refreshRate", Screen.currentResolution.refreshRate);

        int refreshRateIndex = refreshRates.IndexOf(RefreshRate);
        int resolutionIndex = resolutions.IndexOf(resolutions.FirstOrDefault(x => x.width == Width && x.height == Height));

        backgroundMusicToggle.isOn = BackgroundMusic;
        soundEffectsToggle.isOn = SoundEffects;
        shakingEffectToggle.isOn = ShakingEffect;
        resolutionDropdown.value = resolutionIndex;
        refreshRateDropdown.value = refreshRateIndex;
        fullscreenToggle.isOn = Fullscreen;
    }

    private void Update()
    {
        if (Screen.currentResolution.width != Width ||
            Screen.currentResolution.height != Height ||
            Screen.currentResolution.refreshRate != RefreshRate ||
            Screen.fullScreen != Fullscreen ||
            Application.targetFrameRate != RefreshRate)
        {
            Screen.SetResolution(Width, Height, Fullscreen, RefreshRate);
            Application.targetFrameRate = RefreshRate;
        }
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

    public void ToggleFullscreen(bool value)
    {
        Fullscreen = value;
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(Fullscreen));
    }

    public void ChangeResolution(int index)
    {
        int width = resolutions[index].width;
        int height = resolutions[index].height;
        Resolution resolution = resolutions.FirstOrDefault(x => x.width == width && x.height == height);
        Width = resolution.width;
        Height = resolution.height;
        PlayerPrefs.SetInt("resolutionWidth", Width);
        PlayerPrefs.SetInt("resolutionHeight", Height);
        audioSource.Play();
    }

    public void ChangeRefreshRate(int index)
    {
        RefreshRate = refreshRates[index];
        PlayerPrefs.SetInt("refreshRate", RefreshRate);
        audioSource.Play();
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);
        audioSource.Play();
    }
}
