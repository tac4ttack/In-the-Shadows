[System.Serializable]
public class SettingsData
{
    # region Sound Settings
    /* Default values */
    public float DefaultMasterVolume = 1f;
    public float DefaultSFXVolume = 1f;
    public float DefaultMusicVolume = 1f;
    /* Current values */
    public float MasterVolume;
    public float PreviousMasterVolume;
    public float SFXVolume;
    public float PreviousSFXVolume;
    public float MusicVolume;
    public float PreviousMusicVolume;
    public bool SoundMuted;
    # endregion

    public SettingsData()
    {
        MasterVolume = DefaultMasterVolume;
        PreviousMasterVolume = DefaultMasterVolume;
        SFXVolume = DefaultSFXVolume;
        PreviousSFXVolume = DefaultSFXVolume;
        MusicVolume = DefaultMusicVolume;
        PreviousMusicVolume = DefaultMusicVolume;
        SoundMuted = false;
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
    }
}
