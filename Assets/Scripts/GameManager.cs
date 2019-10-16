using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager gm { get; private set; }

    public SettingsData Settings;

    void Awake()
    {
        // Singleton setup
        if (gm == null)
        {
            gm = this;
        }
        else if (gm != this)
        {
            Destroy(gameObject);   
        }
        DontDestroyOnLoad(this.gameObject);

        Settings = SaveSystem.LoadSettings();
        if (Settings == null)
            Settings = new SettingsData();
        Debug.Log("Loaded settings mute is " + Settings.SoundMuted);
    }

    void Start()
    {
        SoundManager.sm.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        SoundManager.sm.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reloading current scene!");
        }
    }
}
