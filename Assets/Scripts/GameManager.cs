using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement; //So you can use SceneManager

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
    public int CurrentSlot = -1;

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
        GameStateMachine.ChangeState(new TitleScreen_GameState(this, GameObject.Find("MainMenu").GetComponent<MainMenu>()));
        
        // Sound settings loading
        SM.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        SM.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;

        // Launch Main Menu Music here
        // Needs to create logic for date formatting and putting it into last played player data
    }

    void Update()
    {
        // DEBUG
        // if (Input.GetKeyUp(KeyCode.R))
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //     Debug.Log("Reloading current scene!");
        // }
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
public class TitleScreen_GameState : IState
{
    private GameManager _GameManager;
    private MainMenu _MainMenu;

    public TitleScreen_GameState(GameManager iGameManager, MainMenu iMainMenu)
    {
        _GameManager = iGameManager;
        _MainMenu = iMainMenu;
    }

    public void Enter()
    {
        // _mainmenu title screen should already be present
        // _MainMenu.TitleScreenCG.gameObject.active = true;
        _GameManager.CurrentState = GameManager.GameStates.TitleScreen;
    }

    public void Execute()
    {
        // if (Input.anyKey)
        // _GameManager.GameStateMachine.ChangeState(new MainMenu_GameState());
    }

    public void Exit()
    {
        // mainmenu title screen pop out
    }
}

public class MainMenu_GameState : IState
{
    public MainMenu_GameState()
    {

    }
    
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class LevelSelection_GameState : IState
{
    public LevelSelection_GameState()
    {

    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class InGame_GameState : IState
{
    public InGame_GameState()
    {

    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class WinScreen_GameState : IState
{
    public WinScreen_GameState()
    {

    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}
#endregion