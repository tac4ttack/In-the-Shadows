using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI MasterVolumeValue;
    public Slider MasterVolumeSlider;
    private float _PrevMasterVolumeValue;

    public TextMeshProUGUI SFXVolumeValue;
    public Slider SFXVolumeSlider;
    private float _PrevSFXVolumeValue;
    
    public TextMeshProUGUI MusicVolumeValue;
    public Slider MusicVolumeSlider;
    private float _PrevMusicVolumeValue;

    public Toggle MuteCheckbox;
    private bool _InitFlag = true;

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
    }

    void Start()
    {
        InitSoundSettings(GameManager.GM.Settings);
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
        _InitFlag = false;
    }

    public void UpdateMasterVolume()
    {
        GameManager.GM.Settings.MasterVolume = MasterVolumeSlider.value;
        MasterVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.MasterVolume * 100f) + "%";
        GameManager.GM.SM.SfxSrc.volume = GameManager.GM.Settings.SFXVolume * GameManager.GM.Settings.MasterVolume;
        GameManager.GM.SM.MusicSrc.volume = GameManager.GM.Settings.MusicVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousMasterVolume = GameManager.GM.Settings.MasterVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateSFXVolume()
    {
        GameManager.GM.Settings.SFXVolume = SFXVolumeSlider.value;
        SFXVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.SFXVolume * 100f) + "%";
        GameManager.GM.SM.SfxSrc.volume = GameManager.GM.Settings.SFXVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.SFXVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateMusicVolume()
    {
        GameManager.GM.Settings.MusicVolume = MusicVolumeSlider.value;
        MusicVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.MusicVolume * 100f) + "%";
        GameManager.GM.SM.MusicSrc.volume = GameManager.GM.Settings.MusicVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.MusicVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void MuteCheckboxToggle()
    {
        if (MuteCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.SoundMuted = true;
            MasterVolumeSlider.interactable = false;
            SFXVolumeSlider.interactable = false;
            MusicVolumeSlider.interactable = false;

            GameManager.GM.Settings.PreviousMasterVolume = GameManager.GM.Settings.MasterVolume;
            MasterVolumeSlider.value = 0f;
            GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.SFXVolume;
            SFXVolumeSlider.value = 0f;
            GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.MusicVolume;
            MusicVolumeSlider.value = 0f;
        }
        else if (!MuteCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.SoundMuted = false;
            MasterVolumeSlider.interactable = true;
            SFXVolumeSlider.interactable = true;
            MusicVolumeSlider.interactable = true;

            MasterVolumeSlider.value = GameManager.GM.Settings.PreviousMasterVolume;
            SFXVolumeSlider.value = GameManager.GM.Settings.PreviousSFXVolume;
            MusicVolumeSlider.value = GameManager.GM.Settings.PreviousMusicVolume;
        }
    }

    public void ResetToDefaultsButtonPress()
    {
        MasterVolumeSlider.value = GameManager.GM.Settings.DefaultMasterVolume;
        GameManager.GM.Settings.PreviousMasterVolume = GameManager.GM.Settings.DefaultMasterVolume;
        SFXVolumeSlider.value = GameManager.GM.Settings.DefaultSFXVolume;
        GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.DefaultSFXVolume;
        MusicVolumeSlider.value = GameManager.GM.Settings.DefaultMusicVolume;
        GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.DefaultMusicVolume;
        GameManager.GM.Settings.SoundMuted = false;
        MasterVolumeSlider.interactable = true;
        SFXVolumeSlider.interactable = true;
        MusicVolumeSlider.interactable = true;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    // DEBUG
    public void TestSoundButtonPress()
    {
        // DEBUG SFX
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[0]);
    }
}