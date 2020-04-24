using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField]
    private GameObject backMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject controlsMenu;
    [SerializeField]
    private Toggle backgroundMusicToggle;
    [SerializeField]
    private Toggle soundEffectsToggle;
    [SerializeField]
    private Toggle shakingEffectToggle;
    [SerializeField]
    private Toggle fullscreenToggle;
    [SerializeField]
    private Toggle autoPauseToggle;
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private TMP_Dropdown refreshRateDropdown;
    [SerializeField]
    private Button okButton;
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button backButton;

    [Header("General Settings")]
    [SerializeField]
    private Dialog dialog;

    private AudioSource audioSource;
    private List<Resolution> resolutions;
    private List<int> refreshRates;

    private SavedOptions options;
    [HideInInspector]
    public bool OptionsChanged;
    private bool changeOptions = true;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        resolutions = Screen.resolutions.Select(x => new Resolution { width = x.width, height = x.height }).Distinct().Reverse().ToList();
        refreshRates = Screen.resolutions.Select(x => x.refreshRate).Distinct().Reverse().ToList();
        resolutions.ForEach(x => resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{x.width} x {x.height}")));
        refreshRates.ForEach(x => refreshRateDropdown.options.Add(new TMP_Dropdown.OptionData($"{x} hz")));

        options = SaveSystem.GetOptions();

        if (options.Resolution.Width == 0 || options.Resolution.Height == 0 || options.Resolution.RefreshRate == 0)
        {
            options.Resolution.Width = Screen.currentResolution.width;
            options.Resolution.Height = Screen.currentResolution.height;
            options.Resolution.RefreshRate = Screen.currentResolution.refreshRate;
        }

        SetOptionValues();
        options.OptionsChanged.AddListener(OptionsUpdated);
        options.Resolution.ResolutionChanged.AddListener(OptionsUpdated);

        Screen.SetResolution(options.Resolution.Width, options.Resolution.Height, options.Fullscreen, options.Resolution.RefreshRate);
        Application.targetFrameRate = options.Resolution.RefreshRate;
        Screen.fullScreen = options.Fullscreen;
    }

    private void OptionsUpdated()
    {
        okButton.gameObject.SetActive(true);
        applyButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        OptionsChanged = true;
    }

    /// <summary>
    /// Toggles the background music
    /// </summary>
    /// <param name="value">If it should be enable or disabled</param>
    public void ToggleBackgroundMusic(bool value)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.BackgroundMusic = value;
    }

    /// <summary>
    /// Toggles the sound effects
    /// </summary>
    /// <param name="value">If it should be enable or disabled</param>
    public void ToggleSoundEffects(bool value)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.SoundEffects = value;
    }

    /// <summary>
    /// Toggles the shaking effect
    /// </summary>
    /// <param name="value">If it should be enable or disabled</param>
    public void ToggleShakingEffect(bool value)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.ShakingEffect = value;
    }

    /// <summary>
    /// Toggles if the game should be in fullscreen
    /// </summary>
    /// <param name="value">If it should be enable or disabled</param>
    public void ToggleFullscreen(bool value)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.Fullscreen = value;
    }

    /// <summary>
    /// Toggles auto pause when focus is lost
    /// </summary>
    /// <param name="value">If it should be enabled or disabled</param>
    public void ToggleAutoPause(bool value)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.AutoPauseOnFocusLose = value;
    }

    /// <summary>
    /// Changes the game resolution
    /// </summary>
    /// <param name="index">The resolution index</param>
    public void ChangeResolution(int index)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        int width = resolutions[index].width;
        int height = resolutions[index].height;
        Resolution resolution = resolutions.FirstOrDefault(x => x.width == width && x.height == height);
        options.Resolution.Width = resolution.width;
        options.Resolution.Height = resolution.height;
    }

    /// <summary>
    /// Changes the game refresh rate
    /// </summary>
    /// <param name="index">The refresh rate index</param>
    public void ChangeRefreshRate(int index)
    {
        if (!changeOptions)
            return;

        audioSource.Play();
        options.Resolution.RefreshRate = refreshRates[index];
    }

    /// <summary>
    /// Applies and saves the changes and goes back
    /// </summary>
    public void Ok()
    {
        Apply();
        Back();
    }

    /// <summary>
    /// Applies and saves the changes
    /// </summary>
    public void Apply()
    {
        audioSource.Play();
        SaveSystem.SaveOptions(options);
        OptionsChanged = false;
        Screen.SetResolution(options.Resolution.Width, options.Resolution.Height, options.Fullscreen, options.Resolution.RefreshRate);
        Application.targetFrameRate = options.Resolution.RefreshRate;

        if (Game.Instance != null)
            Game.Instance.Options = SaveSystem.GetOptions();

        okButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cancels the changes
    /// </summary>
    public void Cancel(bool goBack)
    {
        audioSource.Play();

        if (goBack && OptionsChanged)
        {
            dialog.OnResult += (Dialog.DialogResult result) =>
            {
                if (result == Dialog.DialogResult.Yes)
                {
                    options = SaveSystem.GetOptions();
                    options.OptionsChanged.AddListener(OptionsUpdated);
                    options.Resolution.ResolutionChanged.AddListener(OptionsUpdated);

                    changeOptions = false;
                    SetOptionValues();

                    OptionsChanged = false;
                    changeOptions = true;
                    okButton.gameObject.SetActive(false);
                    applyButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(true);

                    Back();
                }
            };

            dialog.Open(Dialog.DialogType.YesNo, "Are you sure that you want to cancel the changes and go back?");
        }
        else
        {
            if (OptionsChanged)
            {
                options = SaveSystem.GetOptions();
                options.OptionsChanged.AddListener(OptionsUpdated);
                options.Resolution.ResolutionChanged.AddListener(OptionsUpdated);

                changeOptions = false;
                SetOptionValues();

                OptionsChanged = false;
                changeOptions = true;
                okButton.gameObject.SetActive(false);
                applyButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(true);
            }

            if (goBack)
                Back();
        }
    }

    private void SetOptionValues()
    {
        int refreshRateIndex = refreshRates.IndexOf(options.Resolution.RefreshRate);
        int resolutionIndex = resolutions.IndexOf(resolutions.FirstOrDefault(x => x.width == options.Resolution.Width && x.height == options.Resolution.Height));

        backgroundMusicToggle.isOn = options.BackgroundMusic;
        soundEffectsToggle.isOn = options.SoundEffects;
        shakingEffectToggle.isOn = options.ShakingEffect;
        resolutionDropdown.value = resolutionIndex;
        refreshRateDropdown.value = refreshRateIndex;
        fullscreenToggle.isOn = options.Fullscreen;
        autoPauseToggle.isOn = options.AutoPauseOnFocusLose;
    }

    /// <summary>
    /// Opens the controls menu
    /// </summary>
    public void Controls()
    {
        audioSource.Play();

        if (OptionsChanged)
        {
            dialog.OnResult += (Dialog.DialogResult result) =>
            {
                if (result == Dialog.DialogResult.Yes)
                {
                    Cancel(false);
                    optionsMenu.SetActive(false);
                    controlsMenu.SetActive(true);
                }
            };

            dialog.Open(Dialog.DialogType.YesNo, "Are you sure that you want to cancel the changes and open the controls?");
        }
        else
        {
            optionsMenu.SetActive(false);
            controlsMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Goes back to the pause or game menu
    /// </summary>
    private void Back()
    {
        audioSource.Play();
        optionsMenu.SetActive(false);
        backMenu.SetActive(true);
    }
}
