using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager gm { get; private set; }
    public SoundManager soundManager;

    public bool _InDebugMode = false; // replace by private
    public int _CurrentSlot = -1; // replace by private

    public SettingsData Settings;
    public PlayersData Players;

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

        if (!soundManager)
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        Assert.IsNotNull(soundManager, "SoundManager not found in scene!");

        Settings = SaveSystem.LoadSettings();
        if (Settings == null)
            Settings = new SettingsData();

        Players = SaveSystem.LoadPlayers();
        if (Players == null)
            Players = new PlayersData();
    }

    void Start()
    {
        soundManager.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        soundManager.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
        // Launch Main Menu Music here
        // Needs to create logic for date formatting and putting it into last played player data
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Reloading current scene!");
        }
    }

    public void ClearAllPlayersData()
    {
        Players = new PlayersData();
        SaveSystem.SavePlayers(Players);
    }

    public void ClearTargetPlayerData(int iSlot)
    {
        Players.ResetTargetPlayer(iSlot);
        SaveSystem.SavePlayers(Players);
    }

    public void CreateNewPlayer(int iSlot, string iName, bool iDoTutorial)
    {
        if (iSlot >= 0 && iSlot <= 2)
        {
            Players.PlayersName[iSlot] = iName;
            Players.IsEmpty[iSlot] = false;
            Players.DoTutorial[iSlot] = iDoTutorial;
            if (iDoTutorial)
                Players.Progression[iSlot].Level[0] = 1;
            else
                Players.Progression[iSlot].Level[0] = 2;
            Players.Progression[iSlot].Level[1] = 1;
            for (int i = 2; i < Players.Progression[iSlot].Level.Length; i++)
                Players.Progression[iSlot].Level[i] = 0;
            SaveSystem.SavePlayers(Players);
        }
    }

    // DEBUG AND TESTING
    IEnumerator JumpToScene(Scene iTarget)
    {
        yield return null;
    }    
    
    public void TestPlayerSave()
    {
        Debug.Log("DEBUG: Saving players data!");
        SaveSystem.SavePlayers(Players);
    }
}