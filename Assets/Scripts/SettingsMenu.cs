using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    //Default Sliders values
    private float _defaultMasterVolume = 1f;
    private float _defaultSFXVolume = 0.5f;
    private float _defaultMusicVolume = 0.5f;
    
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
    private bool _muted = false;

    void Start()
    {
        // DEBUG
        Debug.Log("Settings Panel START!");

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

        UpdateMasterVolume();
        UpdateSFXVolume();
        UpdateMusicVolume();
        MuteCheckbox.isOn = _muted;
    }
    
    public void UpdateMasterVolume()
    {
        MasterVolumeValue.text = Mathf.RoundToInt(MasterVolumeSlider.value * 100f) + "%";
        SoundManager.sm.SfxSrc.volume = SFXVolumeSlider.value * MasterVolumeSlider.value;
        SoundManager.sm.MusicSrc.volume = MusicVolumeSlider.value * MasterVolumeSlider.value;
        if (!_muted)
            _prevMasterVolumeValue = MasterVolumeSlider.value;        
    }

    public void UpdateSFXVolume()
    {
        SFXVolumeValue.text = Mathf.RoundToInt(SFXVolumeSlider.value * 100f) + "%";
        SoundManager.sm.SfxSrc.volume = SFXVolumeSlider.value * MasterVolumeSlider.value;
        if (!_muted)
            _prevSFXVolumeValue = SFXVolumeSlider.value;
    }

    public void UpdateMusicVolume()
    {
        MusicVolumeValue.text = Mathf.RoundToInt(MusicVolumeSlider.value * 100f) + "%";
        SoundManager.sm.MusicSrc.volume = MusicVolumeSlider.value * MasterVolumeSlider.value;
        if (!_muted)
            _prevMusicVolumeValue = MusicVolumeSlider.value;
    }

    public void MuteCheckboxToggle()
    {
        _muted = !_muted;
        MasterVolumeSlider.interactable = !MasterVolumeSlider.interactable;
        SFXVolumeSlider.interactable = !SFXVolumeSlider.interactable;
        MusicVolumeSlider.interactable = !MusicVolumeSlider.interactable;
        if (_muted)
        {
            MasterVolumeSlider.value = 0f;
            SFXVolumeSlider.value = 0f;
            MusicVolumeSlider.value = 0f;
        }
        else
        {
            MasterVolumeSlider.value = _prevMasterVolumeValue;
            SFXVolumeSlider.value = _prevSFXVolumeValue;
            MusicVolumeSlider.value = _prevMusicVolumeValue;
        }
    }

    public void ResetToDefaults()
    {
        MasterVolumeSlider.value = _defaultMasterVolume;
        SFXVolumeSlider.value = _defaultSFXVolume;
        MusicVolumeSlider.value = _defaultMusicVolume;
        _muted = false;
    }
}
