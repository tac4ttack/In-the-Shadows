using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PlayPanel;
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    private GameObject _activePanel;

    void Start()
    {
        MainPanel.SetActive(true);
        PlayPanel.SetActive(true);
        SettingsPanel.SetActive(true);
        CreditsPanel.SetActive(true);
        if (!MainPanel)
            MainPanel = GameObject.Find("MainPanel");
        if (!PlayPanel)
            PlayPanel = GameObject.Find("PlayPanel");
        if (!SettingsPanel)
            SettingsPanel = GameObject.Find("SettingsPanel");
        if (!CreditsPanel)
            CreditsPanel = GameObject.Find("CreditsPanel");

        MainPanel.SetActive(true);
        PlayPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        _activePanel = MainPanel;

        Assert.IsNotNull(MainPanel, "Main panel GameObject not set!");
        Assert.IsNotNull(PlayPanel, "Play panel GameObject not set!");
        Assert.IsNotNull(SettingsPanel, "Settings panel GameObject not set!");
        Assert.IsNotNull(CreditsPanel, "Credits panel GameObject not set!");

        // DEBUG Music
        GameManager.gm.soundManager.MusicSrc.PlayOneShot(GameManager.gm.soundManager.Musics[0]);
    }

    public void ExitButton()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }

    public void BackButtonPress()
    {
        _activePanel.SetActive(false);
        MainPanel.SetActive(true);
        _activePanel = MainPanel;
    }

    public void PlayButtonPress()
    {
        MainPanel.SetActive(false);
        PlayPanel.SetActive(true);
        _activePanel = PlayPanel;
    }

    public void SettingsButtonPress()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        _activePanel = SettingsPanel;
    }

    public void CreditsButtonPress()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
        _activePanel = CreditsPanel;
    }

    // DEBUG
    public void TestSoundButtonPress()
    {
        // DEBUG SFX
        GameManager.gm.soundManager.SfxSrc.PlayOneShot(GameManager.gm.soundManager.Sfx[0]);
    }
}
