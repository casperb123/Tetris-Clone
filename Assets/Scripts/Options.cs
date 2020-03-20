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

    private bool initiated;
    private AudioSource audioSource;
    private List<Resolution> resolutions;
    private List<int> refreshRates;

    public bool BackgroundMusic { get; private set; }

    public bool SoundEffects { get; private set; }

    public bool ShakingEffect { get; private set; }

    public bool Fullscreen { get; private set; }

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        resolutions = Screen.resolutions.Select(x => new Resolution { width = x.width, height = x.height }).Distinct().Reverse().ToList();
        refreshRates = Screen.resolutions.Select(x => x.refreshRate).Distinct().Reverse().ToList();
        resolutions.ForEach(x => resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{x.width} x {x.height}")));
        refreshRates.ForEach(x => refreshRateDropdown.options.Add(new TMP_Dropdown.OptionData($"{x} hz")));

        BackgroundMusic = PlayerPrefs.HasKey("backgroundMusic") ? Convert.ToBoolean(PlayerPrefs.GetInt("backgroundMusic")) : true;
        SoundEffects = PlayerPrefs.HasKey("soundEffects") ? Convert.ToBoolean(PlayerPrefs.GetInt("soundEffects")) : true;
        ShakingEffect = PlayerPrefs.HasKey("shakingEffect") ? Convert.ToBoolean(PlayerPrefs.GetInt("shakingEffect")) : true;
        Fullscreen = PlayerPrefs.HasKey("fullscreen") ? Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen")) : true;

        int width = PlayerPrefs.HasKey("resolutionWidth") ? PlayerPrefs.GetInt("resolutionWidth") : Screen.currentResolution.width;
        int height = PlayerPrefs.HasKey("resolutionHeight") ? PlayerPrefs.GetInt("resolutionHeight") : Screen.currentResolution.height;
        int refreshRate = PlayerPrefs.HasKey("refreshRate") ? PlayerPrefs.GetInt("refreshRate") : Screen.currentResolution.refreshRate;
        int refreshRateIndex = refreshRates.IndexOf(refreshRate);
        int resolutionIndex = resolutions.IndexOf(resolutions.FirstOrDefault(x => x.width == width && x.height == height));

        Screen.SetResolution(width, height, Fullscreen, refreshRate);

        backgroundMusicToggle.isOn = BackgroundMusic;
        soundEffectsToggle.isOn = SoundEffects;
        shakingEffectToggle.isOn = ShakingEffect;
        resolutionDropdown.value = resolutionIndex;
        refreshRateDropdown.value = refreshRateIndex;
        fullscreenToggle.isOn = Fullscreen;
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
        Resolution currentResolution = Screen.currentResolution;
        Fullscreen = value;
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(Fullscreen));
        Screen.SetResolution(currentResolution.width, currentResolution.height, Fullscreen, currentResolution.refreshRate);
    }

    public void ChangeResolution(int index)
    {
        int width = resolutions[index].width;
        int height = resolutions[index].height;
        Resolution resolution = resolutions.FirstOrDefault(x => x.width == width && x.height == height);
        Resolution currentResolution = Screen.currentResolution;
        PlayerPrefs.SetInt("resolutionWidth", resolution.width);
        PlayerPrefs.SetInt("resolutionHeight", resolution.height);
        Screen.SetResolution(resolution.width, resolution.height, Fullscreen, currentResolution.refreshRate);
    }

    public void ChangeRefreshRate(int index)
    {
        Resolution currentResolution = Screen.currentResolution;
        int refreshRate = refreshRates[index];
        PlayerPrefs.SetInt("refreshRate", refreshRate);
        Screen.SetResolution(currentResolution.width, currentResolution.height, Fullscreen, refreshRate);
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);
        audioSource.Play();
    }
}
