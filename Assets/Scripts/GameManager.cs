using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager GM { get; private set; }
    public SoundManager SM;

    public StateMachine GameStateMachine = new StateMachine();
    public enum GameStates { TitleScreen = 0, MainMenu, LevelSelection, InGame };
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

        CurrentState = GameManager.GameStates.TitleScreen;
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
            if (!iDoTutorial)
                Players.Progression[iSlot].Level[1] = 1;
            for (int i = 2; i < Players.Progression[iSlot].Level.Length; i++)
                Players.Progression[iSlot].Level[i] = 0;
            SaveSystem.SavePlayers(Players);
        }
    }

    public void UpdatePlayersProgressionPercentage()
    {
        int count = 0;

        for (int i = 0; i < 3; i++)
        {
            count = 0;
            foreach (int status in Players.Progression[i].Level)
            {
                if (status == 2)
                    count++;
            }
            Players.ProgressionPercentage[i] = ((float)count / Utility.PuzzleAmount) * 100f;
        }
    }
}

#region Game States
public class InMainMenu_GameState : IState
{
    public InMainMenu_GameState() { }

    public void Enter()
    {
        SceneManager.LoadScene(0);
        GameManager.GM.CurrentState = GameManager.GameStates.MainMenu;
        GameManager.GM.DebugMode = false;
        GameManager.GM.UpdatePlayersProgressionPercentage();
        SaveSystem.SavePlayers(GameManager.GM.Players);
        // add main menu music launch?
    }

    public void Execute() { }

    public void Exit()
    {
        SaveSystem.SavePlayers(GameManager.GM.Players);
    }
}

public class LevelSelection_GameState : IState
{
    public LevelSelection_GameState() { }

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.LevelSelection;
        SceneManager.LoadScene(1);
        SaveSystem.SavePlayers(GameManager.GM.Players);
        // add level selection music launch?
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.Players.LastPlayed[Utility.CurrentPlayer] = System.DateTime.Now.ToString("dd MMM yyyy");
        GameManager.GM.UpdatePlayersProgressionPercentage();
        SaveSystem.SavePlayers(GameManager.GM.Players);
    }
}

public class InGame_GameState : IState
{
    private int _SceneIndex;

    public InGame_GameState(int iTargetSceneIndex)
    {
        _SceneIndex = iTargetSceneIndex;
    }

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.InGame;
        SceneManager.LoadScene(_SceneIndex);
        SaveSystem.SavePlayers(GameManager.GM.Players);
        // add puzzle level music launch depending on the level id?
    }

    public void Execute() { }

    public void Exit()
    {
        GameManager.GM.Players.LastPlayed[Utility.CurrentPlayer] = System.DateTime.Now.ToString("dd MMM yyyy");
        GameManager.GM.Players.LastPlayedLevel[Utility.CurrentPlayer] = _SceneIndex - Utility.LevelSceneIndexOffset;
        GameManager.GM.UpdatePlayersProgressionPercentage();
        SaveSystem.SavePlayers(GameManager.GM.Players);
    }
}

/*
public class EndGame_GameState : IState
{
    public EndGame_GameState() { }

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.EndGame;
        SceneManager.LoadScene(Utility.LevelSceneIndexOffset + GameManager.GM.Players.PuzzlesAmount);
    }

    public void Execute() {}

    public void Exit() {}
}
*/
#endregion