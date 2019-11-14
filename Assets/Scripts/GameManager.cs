﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public int LastPlayedLevel = -1;
    public List<int> ToUnlock;
    public List<int> ToComplete;

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
        
        ToUnlock = new List<int>();
        ToComplete = new List<int>();
    }

    void Start()
    {
        // Sound settings loading
        SM.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        SM.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
        
        CurrentState = GameManager.GameStates.MainMenu;
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
}

#region Game States
public class InMainMenu_GameState : IState
{
    public InMainMenu_GameState() {}
    
    public void Enter()
    {
        SceneManager.LoadScene(0);
        GameManager.GM.CurrentState = GameManager.GameStates.MainMenu;
        GameManager.GM.DebugMode = false;
        GameManager.GM.CurrentPlayerSlot = -1;
        // add main menu music launch?
    }

    public void Execute() {}

    public void Exit() {}
}

public class LevelSelection_GameState : IState
{
    public LevelSelection_GameState() {}

    public void Enter()
    {
        GameManager.GM.CurrentState = GameManager.GameStates.LevelSelection;
        SceneManager.LoadScene(1);
        // add level selection music launch?
    }

    public void Execute() {}

    public void Exit() {}
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
        SceneManager.LoadScene(_SceneIndex + 2);
        // add puzzle level music launch depending on the level id?
    }

    public void Execute() {}

    public void Exit() {}
}
#endregion