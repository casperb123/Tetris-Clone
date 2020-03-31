using System;
using UnityEngine.Events;

[Serializable]
public class SavedOptions
{
    private bool backgroundMusic;
    private bool soundEffects;
    private bool shakingEffect;
    private bool fullscreen;
    private bool autoPauseOnFocusLose;

    public bool BackgroundMusic
    {
        get => backgroundMusic;
        set
        {
            backgroundMusic = value;
            OptionsChanged.Invoke();
        }
    }

    public bool SoundEffects
    {
        get => soundEffects;
        set
        {
            soundEffects = value;
            OptionsChanged.Invoke();
        }
    }

    public bool ShakingEffect
    {
        get => shakingEffect;
        set
        {
            shakingEffect = value;
            OptionsChanged.Invoke();
        }
    }

    public bool Fullscreen
    {
        get => fullscreen;
        set
        {
            fullscreen = value;
            OptionsChanged.Invoke();
        }
    }

    public bool AutoPauseOnFocusLose
    {
        get => autoPauseOnFocusLose;
        set
        {
            autoPauseOnFocusLose = value;
            OptionsChanged.Invoke();
        }
    }

    public SavedResolution Resolution { get; set; }

    [NonSerialized]
    public UnityEvent OptionsChanged;

    public SavedOptions()
    {
        OptionsChanged = new UnityEvent();
        backgroundMusic = true;
        soundEffects = true;
        shakingEffect = true;
        fullscreen = true;
        autoPauseOnFocusLose = true;
        Resolution = new SavedResolution();
    }

    public SavedOptions(SavedOptions options) : this()
    {
        backgroundMusic = options.BackgroundMusic;
        soundEffects = options.SoundEffects;
        shakingEffect = options.ShakingEffect;
        fullscreen = options.Fullscreen;
        autoPauseOnFocusLose = options.AutoPauseOnFocusLose;
        Resolution = new SavedResolution(options.Resolution);
    }
}

[Serializable]
public class SavedResolution
{
    protected int width;
    protected int height;
    protected int refreshRate;

    public int Width
    {
        get => width;
        set
        {
            width = value;
            ResolutionChanged.Invoke();
        }
    }
    public int Height
    {
        get => height;
        set
        {
            height = value;
            ResolutionChanged.Invoke();
        }
    }
    public int RefreshRate
    {
        get => refreshRate;
        set
        {
            refreshRate = value;
            ResolutionChanged.Invoke();
        }
    }

    [NonSerialized]
    public UnityEvent ResolutionChanged;

    public SavedResolution()
    {
        ResolutionChanged = new UnityEvent();
    }

    public SavedResolution(SavedResolution resolution) : this()
    {
        width = resolution.Width;
        height = resolution.Height;
        refreshRate = resolution.RefreshRate;
    }
}