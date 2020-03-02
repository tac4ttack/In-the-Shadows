[System.Serializable]
public class SettingsData
{
    # region Sound Settings
    /* Default values */
    public float DefaultMasterVolume = 1f;
    public float DefaultSFXVolume = 1f;
    public bool DefaultMute = false;
    public bool DefaultFPSCounter = false;
    public bool DefaultFXAA = true;
    public float DefaultMusicVolume = 1f;
    public float DefaultSpeedRotation = 42f;
    public float DefaultSpeedTranslation = 1f;
    /* Current values */
    public float MasterVolume;
    public float PreviousMasterVolume;
    public float SFXVolume;
    public float PreviousSFXVolume;
    public float MusicVolume;
    public float PreviousMusicVolume;
    public bool SoundMuted;
    public bool FPSCounter;
    public bool FXAAEnabled;
    public float SpeedRotation;
    public float SpeedTranslation;
    # endregion

    public SettingsData()
    {
        MasterVolume = DefaultMasterVolume;
        PreviousMasterVolume = DefaultMasterVolume;
        SFXVolume = DefaultSFXVolume;
        PreviousSFXVolume = DefaultSFXVolume;
        MusicVolume = DefaultMusicVolume;
        PreviousMusicVolume = DefaultMusicVolume;
        SoundMuted = DefaultMute;
        FPSCounter = DefaultFPSCounter;
        FXAAEnabled = DefaultFXAA;
        SpeedRotation = DefaultSpeedRotation;
        SpeedTranslation = DefaultSpeedTranslation;
    }

    public SettingsData(SettingsData iData)
    {
        MasterVolume = iData.MasterVolume;
        PreviousMasterVolume = iData.PreviousMasterVolume;
        SFXVolume = iData.SFXVolume;
        PreviousSFXVolume = iData.PreviousSFXVolume;
        MusicVolume = iData.MusicVolume;
        PreviousMusicVolume = iData.PreviousMusicVolume;
        SoundMuted = iData.SoundMuted;
        FPSCounter = iData.FPSCounter;
        FXAAEnabled = iData.FXAAEnabled;
        SpeedRotation = iData.SpeedRotation;
        SpeedTranslation = iData.SpeedTranslation;
    }
}
