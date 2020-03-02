using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private SettingsMenu _SettingsMenu_Script;

    private TextMeshProUGUI _MasterVolumeValue;
    private Slider _MasterVolumeSlider;
    private float _PrevMasterVolumeValue;

    private TextMeshProUGUI _SFXVolumeValue;
    private Slider _SFXVolumeSlider;
    private float _PrevSFXVolumeValue;

    private TextMeshProUGUI _MusicVolumeValue;
    private Slider _MusicVolumeSlider;
    private float _PrevMusicVolumeValue;

    private TextMeshProUGUI _RotationSpeedValue;
    private Slider _RotationSpeedSlider;
    private float _PrevRotationSpeedValue;

    private TextMeshProUGUI _TranslationSpeedValue;
    private Slider _TranslationSpeedSlider;
    private float _PrevTranslationSpeedValue;

    private bool _InitFlag = true;
    private Toggle _MuteCheckbox;
    private Toggle _FPSCounterCheckbox;
    private Toggle _FXAACheckbox;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"SETTINGS MENU - {this.name} - Awake()");
        #endif

        if (_SettingsMenu_Script == null)
            _SettingsMenu_Script = GameObject.FindGameObjectWithTag("Settings/Panel").GetComponent<SettingsMenu>();
        Assert.IsNotNull(_SettingsMenu_Script, "Settings panel with menu script not found!");

        if (_MasterVolumeValue == null)
            _MasterVolumeValue = GameObject.FindGameObjectWithTag("Settings/Master Volume Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_MasterVolumeValue, "Master volume text value component is missing!");

        if (_MasterVolumeSlider == null)
            _MasterVolumeSlider = GameObject.FindGameObjectWithTag("Settings/Master Volume Slider").GetComponent<Slider>();
        Assert.IsNotNull(_MasterVolumeSlider, "Master volume slider is missing!");
        _MasterVolumeSlider.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });

        if (_SFXVolumeValue == null)
            _SFXVolumeValue = GameObject.FindGameObjectWithTag("Settings/SFX Volume Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_SFXVolumeValue, "SFX volume text value component is missing!");

        if (_SFXVolumeSlider == null)
            _SFXVolumeSlider = GameObject.FindGameObjectWithTag("Settings/SFX Volume Slider").GetComponent<Slider>();
        Assert.IsNotNull(_SFXVolumeSlider, "SFX volume slider is missing!");
        _SFXVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });

        if (_MusicVolumeValue == null)
            _MusicVolumeValue = GameObject.FindGameObjectWithTag("Settings/Music Volume Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_MusicVolumeValue, "Music volume text value component is missing!");

        if (_MusicVolumeSlider == null)
            _MusicVolumeSlider = GameObject.FindGameObjectWithTag("Settings/Music Volume Slider").GetComponent<Slider>();
        Assert.IsNotNull(_MusicVolumeSlider, "Music volume slider is missing!");
        _MusicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });

        if (_MuteCheckbox == null)
            _MuteCheckbox = GameObject.FindGameObjectWithTag("Settings/Mute Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_MuteCheckbox, "Mute checkbox is missing!");
        _MuteCheckbox.onValueChanged.AddListener(delegate { MuteCheckboxToggle(); });

        if (_FPSCounterCheckbox == null)
            _FPSCounterCheckbox = GameObject.FindGameObjectWithTag("Settings/FPS Counter Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_MuteCheckbox, "FPS Counter checkbox is missing!");
        _FPSCounterCheckbox.onValueChanged.AddListener(delegate { FPSCounterCheckboxToggle(); });

        if (_FXAACheckbox == null)
            _FXAACheckbox = GameObject.FindGameObjectWithTag("Settings/FXAA Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_FXAACheckbox, "FXAA checkbox is missing!");
        _FXAACheckbox.onValueChanged.AddListener(delegate { FXAACheckboxToggle(); });

        if (_RotationSpeedValue == null)
            _RotationSpeedValue = GameObject.FindGameObjectWithTag("Settings/Rotation Speed Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_RotationSpeedValue, "Rotation speed text value component is missing!");

        if (_RotationSpeedSlider == null)
            _RotationSpeedSlider = GameObject.FindGameObjectWithTag("Settings/Rotation Speed Slider").GetComponent<Slider>();
        Assert.IsNotNull(_RotationSpeedSlider, "Rotation speed slider is missing!");
        _RotationSpeedSlider.onValueChanged.AddListener(delegate { UpdateRotationSpeed(); });

        if (_TranslationSpeedValue == null)
            _TranslationSpeedValue = GameObject.FindGameObjectWithTag("Settings/Translation Speed Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_TranslationSpeedValue, "Rotation speed text value component is missing!");

        if (_TranslationSpeedSlider == null)
            _TranslationSpeedSlider = GameObject.FindGameObjectWithTag("Settings/Translation Speed Slider").GetComponent<Slider>();
        Assert.IsNotNull(_TranslationSpeedSlider, "Translation speed slider is missing!");
        _TranslationSpeedSlider.onValueChanged.AddListener(delegate { UpdateTranslationSpeed(); });
    }

    void Start()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"SETTINGS MENU - {this.name} - Start()");
        #endif

        InitSoundSettings(GameManager.GM.Settings);
        InitGeneralSettings(GameManager.GM.Settings);
        GameManager.GM.PM.InitCamAntialiasing();
        _InitFlag = false;
    }

    private void InitSoundSettings(SettingsData iData)
    {
        _MasterVolumeSlider.value = iData.MasterVolume;
        _SFXVolumeSlider.value = iData.SFXVolume;
        _MusicVolumeSlider.value = iData.MusicVolume;
        _MuteCheckbox.isOn = iData.SoundMuted;
        _MasterVolumeSlider.interactable = !iData.SoundMuted;
        _SFXVolumeSlider.interactable = !iData.SoundMuted;
        _MusicVolumeSlider.interactable = !iData.SoundMuted;
    }

    private void InitGeneralSettings(SettingsData iData)
    {
        _FPSCounterCheckbox.isOn = iData.FPSCounter;
        _FXAACheckbox.isOn = iData.FXAAEnabled;
        _RotationSpeedSlider.value = iData.SpeedRotation;
        _TranslationSpeedSlider.value = iData.SpeedTranslation;
    }

    public void UpdateMasterVolume()
    {
        GameManager.GM.Settings.MasterVolume = _MasterVolumeSlider.value;
        _MasterVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.MasterVolume * 100f) + "%";
        GameManager.GM.SM.SfxSrc.volume = GameManager.GM.Settings.SFXVolume * GameManager.GM.Settings.MasterVolume;
        GameManager.GM.SM.MusicSrc.volume = GameManager.GM.Settings.MusicVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousMasterVolume = GameManager.GM.Settings.MasterVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateSFXVolume()
    {
        GameManager.GM.Settings.SFXVolume = _SFXVolumeSlider.value;
        _SFXVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.SFXVolume * 100f) + "%";
        GameManager.GM.SM.SfxSrc.volume = GameManager.GM.Settings.SFXVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.SFXVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateMusicVolume()
    {
        GameManager.GM.Settings.MusicVolume = _MusicVolumeSlider.value;
        _MusicVolumeValue.text = Mathf.RoundToInt(GameManager.GM.Settings.MusicVolume * 100f) + "%";
        GameManager.GM.SM.MusicSrc.volume = GameManager.GM.Settings.MusicVolume * GameManager.GM.Settings.MasterVolume;
        if (!GameManager.GM.Settings.SoundMuted)
            GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.MusicVolume;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateRotationSpeed()
    {
        GameManager.GM.Settings.SpeedRotation = _RotationSpeedSlider.value;
        _RotationSpeedValue.text = _RotationSpeedSlider.value.ToString("0.00");
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void UpdateTranslationSpeed()
    {
        GameManager.GM.Settings.SpeedTranslation = _TranslationSpeedSlider.value;
        _TranslationSpeedValue.text = _TranslationSpeedSlider.value.ToString("0.00");
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void MuteCheckboxToggle()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);
        if (_MuteCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.SoundMuted = true;
            _MasterVolumeSlider.interactable = false;
            _SFXVolumeSlider.interactable = false;
            _MusicVolumeSlider.interactable = false;

            _MasterVolumeSlider.value = 0f;
            _SFXVolumeSlider.value = 0f;
            _MusicVolumeSlider.value = 0f;
        }
        else if (!_MuteCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.SoundMuted = false;
            _MasterVolumeSlider.interactable = true;
            _SFXVolumeSlider.interactable = true;
            _MusicVolumeSlider.interactable = true;

            _MasterVolumeSlider.value = GameManager.GM.Settings.PreviousMasterVolume;
            _SFXVolumeSlider.value = GameManager.GM.Settings.PreviousSFXVolume;
            _MusicVolumeSlider.value = GameManager.GM.Settings.PreviousMusicVolume;
        }
    }

    public void FPSCounterCheckboxToggle()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);
        if (_FPSCounterCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.FPSCounter = true;
        }
        else if (!_FPSCounterCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.FPSCounter = false;
        }
    }

    public void FXAACheckboxToggle()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);
        if (_FXAACheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.FXAAEnabled = true;
        }
        else if (!_FXAACheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.FXAAEnabled = false;
        }
        GameManager.GM.PM.InitCamAntialiasing();
    }

    public void ResetToDefaultsButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);

        _MasterVolumeSlider.value = GameManager.GM.Settings.DefaultMasterVolume;
        GameManager.GM.Settings.PreviousMasterVolume = GameManager.GM.Settings.DefaultMasterVolume;
        _MasterVolumeSlider.interactable = true;
        
        _SFXVolumeSlider.value = GameManager.GM.Settings.DefaultSFXVolume;
        GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.DefaultSFXVolume;
        _SFXVolumeSlider.interactable = true;
        
        _MusicVolumeSlider.value = GameManager.GM.Settings.DefaultMusicVolume;
        GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.DefaultMusicVolume;
        _MusicVolumeSlider.interactable = true;

        GameManager.GM.Settings.SoundMuted = false;
        _MuteCheckbox.isOn = false;

        GameManager.GM.Settings.FXAAEnabled = true;
        _FXAACheckbox.isOn = true;

        GameManager.GM.Settings.FPSCounter = false;
        _FPSCounterCheckbox.isOn = false;

        _RotationSpeedSlider.value = GameManager.GM.Settings.DefaultSpeedRotation;
        _TranslationSpeedSlider.value = GameManager.GM.Settings.DefaultSpeedTranslation;

        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void TestSoundButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[3]);
    }
}