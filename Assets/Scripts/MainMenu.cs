using UnityEngine;
using UnityEngine.Assertions;

public class MainMenu : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject PlayPanel;
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    private GameObject _activePanel;

    void Start()
    {
        Assert.IsNotNull(MainPanel, "Main panel GameObject not set!");
        Assert.IsNotNull(PlayPanel, "Play panel GameObject not set!");
        Assert.IsNotNull(SettingsPanel, "Settings panel GameObject not set!");
        Assert.IsNotNull(CreditsPanel, "Credits panel GameObject not set!");

        MainPanel.SetActive(true);
        PlayPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        _activePanel = MainPanel;
    }

    public void ExitButton()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }

    public void BackButton()
    {
        _activePanel.SetActive(false);
        MainPanel.SetActive(true);
        _activePanel = MainPanel;
    }

    public void PlayButton()
    {
        MainPanel.SetActive(false);
        PlayPanel.SetActive(true);
        _activePanel = PlayPanel;
    }

    public void SettingsButton()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        _activePanel = SettingsPanel;
    }

    public void CreditsButton()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
        _activePanel = CreditsPanel;
    }
}
