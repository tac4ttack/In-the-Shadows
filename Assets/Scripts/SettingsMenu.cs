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

    private bool _InitFlag = true;
    private Toggle _MuteCheckbox;
    private Toggle _FPSCounterCheckbox;
    private Toggle _MouseControlsCheckbox;
    private Toggle _FXAACheckbox;

    void Awake()
    {
        if (_SettingsMenu_Script == null)
            _SettingsMenu_Script = GameObject.FindGameObjectWithTag("Settings_Panel").GetComponent<SettingsMenu>();
        Assert.IsNotNull(_SettingsMenu_Script, "Settings panel with menu script not found!");

        if (_MasterVolumeValue == null)
            _MasterVolumeValue = GameObject.FindGameObjectWithTag("Settings_MasterVolume_Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_MasterVolumeValue, "Master volume text value component is missing!");

        if (_MasterVolumeSlider == null)
            _MasterVolumeSlider = GameObject.FindGameObjectWithTag("Settings_MasterVolume_Slider").GetComponent<Slider>();
        Assert.IsNotNull(_MasterVolumeSlider, "Master volume slider is missing!");
        _MasterVolumeSlider.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });

        if (_SFXVolumeValue == null)
            _SFXVolumeValue = GameObject.FindGameObjectWithTag("Settings_SFXVolume_Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_SFXVolumeValue, "SFX volume text value component is missing!");

        if (_SFXVolumeSlider == null)
            _SFXVolumeSlider = GameObject.FindGameObjectWithTag("Settings_SFXVolume_Slider").GetComponent<Slider>();
        Assert.IsNotNull(_SFXVolumeSlider, "SFX volume slider is missing!");
        _SFXVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });

        if (_MusicVolumeValue == null)
            _MusicVolumeValue = GameObject.FindGameObjectWithTag("Settings_MusicVolume_Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_MusicVolumeValue, "Music volume text value component is missing!");

        if (_MusicVolumeSlider == null)
            _MusicVolumeSlider = GameObject.FindGameObjectWithTag("Settings_MusicVolume_Slider").GetComponent<Slider>();
        Assert.IsNotNull(_MusicVolumeSlider, "Music volume slider is missing!");
        _MusicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });

        if (_MuteCheckbox == null)
            _MuteCheckbox = GameObject.FindGameObjectWithTag("Settings_Mute_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_MuteCheckbox, "Mute checkbox is missing!");
        _MuteCheckbox.onValueChanged.AddListener(delegate { MuteCheckboxToggle(); });

        if (_FPSCounterCheckbox == null)
            _FPSCounterCheckbox = GameObject.FindGameObjectWithTag("Settings_FPSCounter_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_MuteCheckbox, "FPS Counter checkbox is missing!");
        _MuteCheckbox.onValueChanged.AddListener(delegate { FPSCounterCheckboxToggle(); });

        if (_MouseControlsCheckbox == null)
            _MouseControlsCheckbox = GameObject.FindGameObjectWithTag("Settings_MouseControls_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_MouseControlsCheckbox, "Mouse controls checkbox is missing!");
        _MouseControlsCheckbox.onValueChanged.AddListener(delegate { MouseControlsCheckboxToggle(); });

        if (_FXAACheckbox == null)
            _FXAACheckbox = GameObject.FindGameObjectWithTag("Settings_FXAA_Checkbox").GetComponent<Toggle>();
        Assert.IsNotNull(_FXAACheckbox, "FXAA checkbox is missing!");
        _FXAACheckbox.onValueChanged.AddListener(delegate { FXAACheckboxToggle(); });
    }

    void Start()
    {
        InitSoundSettings(GameManager.GM.Settings);
        InitGeneralSettings(GameManager.GM.Settings);
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
        _MouseControlsCheckbox.isOn = iData.MouseControls;
        _FXAACheckbox.isOn = iData.FXAAEnabled;
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

    public void MuteCheckboxToggle()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);
        if (_MuteCheckbox.isOn && !_InitFlag)
        {
            Debug.Log("Mute switched to ON!");
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
            Debug.Log("Mute switched to OFF!");
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

    public void MouseControlsCheckboxToggle()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[2]);
        if (_MouseControlsCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.MouseControls = true;
        }
        else if (!_MouseControlsCheckbox.isOn && !_InitFlag)
        {
            GameManager.GM.Settings.MouseControls = false;
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
        _SFXVolumeSlider.value = GameManager.GM.Settings.DefaultSFXVolume;
        GameManager.GM.Settings.PreviousSFXVolume = GameManager.GM.Settings.DefaultSFXVolume;
        _MusicVolumeSlider.value = GameManager.GM.Settings.DefaultMusicVolume;
        GameManager.GM.Settings.PreviousMusicVolume = GameManager.GM.Settings.DefaultMusicVolume;
        GameManager.GM.Settings.SoundMuted = false;
        _MasterVolumeSlider.interactable = true;
        _SFXVolumeSlider.interactable = true;
        _MusicVolumeSlider.interactable = true;
        SaveSystem.SaveSettings(GameManager.GM.Settings);
    }

    public void TestSoundButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[3]);
    }
}