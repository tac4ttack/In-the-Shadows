﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using ITS.SoundManagement;
using ITS.PostProcessManagement;
using ITS.StateMachinery;
using ITS.Data;
using ITS.SavingSystem;
using ITS.SharedUtility;

namespace ITS.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector] public static GameManager GM { get; private set; }
        public SoundManager SM;
        public PostProcessManager PM;

        public StateMachine GameStateMachine = new StateMachine();
        public enum GameStates { TitleScreen = 0, MainMenu, LevelSelection, InGame };
        public GameStates CurrentState;

        public SettingsData Settings;
        public PlayersData Players;

        public bool DebugMode = false;
        public int CurrentPlayerSlot = -1;

        void Awake()
        {
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"GAME MANAGER - {this.name} - Awake()");
            #endif

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
            Assert.IsNotNull(SM, "Sound Manager not found!");

            if (PM == null)
                PM = this.GetComponent<PostProcessManager>();
            Assert.IsNotNull(PM, "Postprocess Manager not found!");

            Settings = SaveSystem.LoadSettings();
            if (Settings == null)
                Settings = new SettingsData();

            Players = SaveSystem.LoadPlayers();
            if (Players == null)
                Players = new PlayersData();

            QualitySettings.SetQualityLevel(0);

            CurrentState = GameManager.GameStates.TitleScreen;
        }

        void Start()
        {
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"GAME MANAGER - {this.name} - Start()");
            #endif

            // Sound settings loading
            SM.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
            SM.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
            GameManager.GM.SM.PlayMusic(GameManager.GM.SM.Musics[0]);
        }

        public void ClearAllPlayersData()
        {
            Players = new PlayersData();
            SaveSystem.SavePlayer(Players);
        }

        public void ClearTargetPlayerData(int iSlot)
        {
            Players.ResetTargetPlayer(iSlot);
            SaveSystem.SavePlayer(Players);
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
                SaveSystem.SavePlayer(Players);
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                QualitySettings.IncreaseLevel();
                // DEBUG
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log(QualitySettings.GetQualityLevel());
                #endif
            }   
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                QualitySettings.DecreaseLevel();
                // DEBUG
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log(QualitySettings.GetQualityLevel());
                #endif
            }
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Input.GetKeyDown(KeyCode.Home))
                Debug.Log(QualitySettings.GetQualityLevel());
            #endif
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
            GameManager.GM.SM.MusicSrc.Stop();
            GameManager.GM.SM.PlayMusic(GameManager.GM.SM.Musics[0]);
        }

        public void Execute() { }

        public void Exit()
        {
            SaveSystem.SavePlayer(GameManager.GM.Players);
            GameManager.GM.SM.MusicSrc.Stop();
        }
    }

    public class LevelSelection_GameState : IState
    {
        public LevelSelection_GameState() { }

        public void Enter()
        {
            GameManager.GM.CurrentState = GameManager.GameStates.LevelSelection;
            SceneManager.LoadScene(1);
            GameManager.GM.SM.MusicSrc.Stop();
            GameManager.GM.SM.PlayMusic(GameManager.GM.SM.Musics[1]);
        }

        public void Execute() { }

        public void Exit()
        {
            GameManager.GM.Players.LastPlayed[Utility.CurrentPlayer] = System.DateTime.Now.ToString("dd MMM yyyy");
            GameManager.GM.UpdatePlayersProgressionPercentage();
            SaveSystem.SavePlayer(GameManager.GM.Players);
            GameManager.GM.SM.MusicSrc.Stop();
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
            GameManager.GM.SM.MusicSrc.Stop();
            GameManager.GM.SM.PlayMusic(GameManager.GM.SM.Musics[_SceneIndex]);
        }

        public void Execute() { }

        public void Exit()
        {
            GameManager.GM.Players.LastPlayed[Utility.CurrentPlayer] = System.DateTime.Now.ToString("dd MMM yyyy");
            GameManager.GM.Players.LastPlayedLevel[Utility.CurrentPlayer] = _SceneIndex - Utility.LevelSceneIndexOffset;
            GameManager.GM.UpdatePlayersProgressionPercentage();
            SaveSystem.SavePlayer(GameManager.GM.Players);
            GameManager.GM.SM.MusicSrc.Stop();
        }
    }
    #endregion
}
