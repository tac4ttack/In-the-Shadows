using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI MasterVolumeValue;
    public Slider MasterVolumeSlider;
    private float _prevMasterVolumeValue;

    public TextMeshProUGUI SFXVolumeValue;
    public Slider SFXVolumeSlider;
    private float _prevSFXVolumeValue;
    
    public TextMeshProUGUI MusicVolumeValue;
    public Slider MusicVolumeSlider;
    private float _prevMusicVolumeValue;

    public Toggle MuteCheckbox;
    private bool _initFlag = true;

    void Awake()
    {
        if (!MasterVolumeValue)
            MasterVolumeValue = GameObject.Find("MasterVolume_Value").GetComponent<TextMeshProUGUI>();
        if (!MasterVolumeSlider)
            MasterVolumeSlider = GameObject.Find("MasterVolume_Slider").GetComponent<Slider>();

        if (!SFXVolumeValue)
            SFXVolumeValue = GameObject.Find("SFXVolume_Value").GetComponent<TextMeshProUGUI>();
        if (!SFXVolumeSlider)
            SFXVolumeSlider = GameObject.Find("SFXVolume_Slider").GetComponent<Slider>();

        if (!MusicVolumeValue)
            MusicVolumeValue = GameObject.Find("MusicVolume_Value").GetComponent<TextMeshProUGUI>();
        if (!MusicVolumeSlider)
            MusicVolumeSlider = GameObject.Find("MusicVolume_Slider").GetComponent<Slider>();

        if (!MuteCheckbox)
            MuteCheckbox = GameObject.Find("Mute_Checkbox").GetComponent<Toggle>();

        Assert.IsNotNull(MasterVolumeValue, "Master volume text value component is missing!");
        Assert.IsNotNull(MasterVolumeSlider, "Master volume slider is missing!");

        Assert.IsNotNull(SFXVolumeValue, "SFX volume text value component is missing!");
        Assert.IsNotNull(SFXVolumeSlider, "SFX volume slider is missing!");

        Assert.IsNotNull(MusicVolumeValue, "Music volume text value component is missing!");
        Assert.IsNotNull(MusicVolumeSlider, "Music volume slider is missing!");

        Assert.IsNotNull(MuteCheckbox, "Mute checkbox is missing!");
        
        InitSoundSettings(GameManager.gm.Settings);
    }

    public void InitSoundSettings(SettingsData iData)
    {
        MasterVolumeSlider.value = iData.MasterVolume;
        SFXVolumeSlider.value = iData.SFXVolume;
        MusicVolumeSlider.value = iData.MusicVolume;
        MuteCheckbox.isOn = iData.SoundMuted;
        MasterVolumeSlider.interactable = !iData.SoundMuted;
        SFXVolumeSlider.interactable = !iData.SoundMuted;
        MusicVolumeSlider.interactable = !iData.SoundMuted;
        _initFlag = false;
    }

    public void UpdateMasterVolume()
    {
        GameManager.gm.Settings.MasterVolume = MasterVolumeSlider.value;
        MasterVolumeValue.text = Mathf.RoundToInt(GameManager.gm.Settings.MasterVolume * 100f) + "%";
        SoundManager.sm.SfxSrc.volume = GameManager.gm.Settings.SFXVolume * GameManager.gm.Settings.MasterVolume;
        SoundManager.sm.MusicSrc.volume = GameManager.gm.Settings.MusicVolume * GameManager.gm.Settings.MasterVolume;
        if (!GameManager.gm.Settings.SoundMuted)
            GameManager.gm.Settings.PreviousMasterVolume = GameManager.gm.Settings.MasterVolume;
        SaveSystem.SaveSettings(GameManager.gm.Settings);
    }

    public void UpdateSFXVolume()
    {
        GameManager.gm.Settings.SFXVolume = SFXVolumeSlider.value;
        SFXVolumeValue.text = Mathf.RoundToInt(GameManager.gm.Settings.SFXVolume * 100f) + "%";
        SoundManager.sm.SfxSrc.volume = GameManager.gm.Settings.SFXVolume * GameManager.gm.Settings.MasterVolume;
        if (!GameManager.gm.Settings.SoundMuted)
            GameManager.gm.Settings.PreviousSFXVolume = GameManager.gm.Settings.SFXVolume;
        SaveSystem.SaveSettings(GameManager.gm.Settings);
    }

    public void UpdateMusicVolume()
    {
        GameManager.gm.Settings.MusicVolume = MusicVolumeSlider.value;
        MusicVolumeValue.text = Mathf.RoundToInt(GameManager.gm.Settings.MusicVolume * 100f) + "%";
        SoundManager.sm.MusicSrc.volume = GameManager.gm.Settings.MusicVolume * GameManager.gm.Settings.MasterVolume;
        if (!GameManager.gm.Settings.SoundMuted)
            GameManager.gm.Settings.PreviousMusicVolume = GameManager.gm.Settings.MusicVolume;
        SaveSystem.SaveSettings(GameManager.gm.Settings);
    }

    public void MuteCheckboxToggle()
    {
        if (MuteCheckbox.isOn && !_initFlag)
        {
            GameManager.gm.Settings.SoundMuted = true;
            MasterVolumeSlider.interactable = false;
            SFXVolumeSlider.interactable = false;
            MusicVolumeSlider.interactable = false;

            GameManager.gm.Settings.PreviousMasterVolume = GameManager.gm.Settings.MasterVolume;
            MasterVolumeSlider.value = 0f;
            GameManager.gm.Settings.PreviousSFXVolume = GameManager.gm.Settings.SFXVolume;
            SFXVolumeSlider.value = 0f;
            GameManager.gm.Settings.PreviousMusicVolume = GameManager.gm.Settings.MusicVolume;
            MusicVolumeSlider.value = 0f;
        }
        else if (!MuteCheckbox.isOn && !_initFlag)
        {
            GameManager.gm.Settings.SoundMuted = false;
            MasterVolumeSlider.interactable = true;
            SFXVolumeSlider.interactable = true;
            MusicVolumeSlider.interactable = true;

            MasterVolumeSlider.value = GameManager.gm.Settings.PreviousMasterVolume;
            SFXVolumeSlider.value = GameManager.gm.Settings.PreviousSFXVolume;
            MusicVolumeSlider.value = GameManager.gm.Settings.PreviousMusicVolume;
        }
    }

    public void ResetToDefaults()
    {
        MasterVolumeSlider.value = GameManager.gm.Settings.DefaultMasterVolume;
        GameManager.gm.Settings.PreviousMasterVolume = GameManager.gm.Settings.DefaultMasterVolume;
        SFXVolumeSlider.value = GameManager.gm.Settings.DefaultSFXVolume;
        GameManager.gm.Settings.PreviousSFXVolume = GameManager.gm.Settings.DefaultSFXVolume;
        MusicVolumeSlider.value = GameManager.gm.Settings.DefaultMusicVolume;
        GameManager.gm.Settings.PreviousMusicVolume = GameManager.gm.Settings.DefaultMusicVolume;
        GameManager.gm.Settings.SoundMuted = false;
        MasterVolumeSlider.interactable = true;
        SFXVolumeSlider.interactable = true;
        MusicVolumeSlider.interactable = true;
        SaveSystem.SaveSettings(GameManager.gm.Settings);
    }

    public void BackButton()
    {
        SaveSystem.SaveSettings(GameManager.gm.Settings);
    }
}