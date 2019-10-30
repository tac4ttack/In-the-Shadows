using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager GM { get; private set; }
    public SoundManager SM;

    public StateMachine GameStateMachine = new StateMachine();
    public enum GameStates { TitleScreen = 0, MainMenu, LevelSelection, InGame};
    public GameStates CurrentState;

    public SettingsData Settings;
    public PlayersData Players;

    public bool DebugMode = false;
    public int CurrentPlayerSlot = -1;

    void Awake()
    {
        // Singleton setup
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);   
        }
        DontDestroyOnLoad(this.gameObject);

        if (SM == null)
            SM = this.GetComponent<SoundManager>();
        Assert.IsNotNull(SM, "SoundManager not found!");

        Settings = SaveSystem.LoadSettings();
        if (Settings == null)
            Settings = new SettingsData();

        Players = SaveSystem.LoadPlayers();
        if (Players == null)
            Players = new PlayersData();
    }

    void Start()
    {
        // Sound settings loading
        SM.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        SM.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
        
        GameStateMachine.ChangeState(new InMainMenu_GameState());
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
}

#region Game States
public class InMainMenu_GameState : IState
{
    public InMainMenu_GameState()
    {

    }
    
    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.MainMenu;
        GameManager.GM.DebugMode = false;
        GameManager.GM.CurrentPlayerSlot = -1;
        // add main menu music launch?
    }

    public void Execute()
    {
        // throw new System.NotImplementedException();
    }

    public void Exit()
    {
        // throw new System.NotImplementedException();
        // stop all musics?
    }
}

public class LevelSelection_GameState : IState
{
    public LevelSelection_GameState()
    {

    }

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.LevelSelection;
        SceneManager.LoadScene(1);
        // add level selection music launch?
    }

    public void Execute()
    {
        // throw new System.NotImplementedException();
    }

    public void Exit()
    {
        // throw new System.NotImplementedException();
        // stop all musics?
    }
}

public class InGame_GameState : IState
{
    public InGame_GameState()
    {

    }

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.InGame;
        // add puzzle level music launch depending on the level id?
    }

    public void Execute()
    {
        // throw new System.NotImplementedException();
    }

    public void Exit()
    {
        // throw new System.NotImplementedException();
    }
}
#endregion