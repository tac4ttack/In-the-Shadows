﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager gm { get; private set; }
    
    public bool _inDebugMode = false; // replace by private
    public int _currentSlot = -1; // replace by private

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

        Settings = SaveSystem.LoadSettings();
        if (Settings == null)
            Settings = new SettingsData();

        Players = SaveSystem.LoadPlayers();
        if (Players == null)
            Players = new PlayersData();
    }

    void Start()
    {
        SoundManager.sm.SfxSrc.volume = Settings.SFXVolume * Settings.MasterVolume;
        SoundManager.sm.MusicSrc.volume = Settings.MusicVolume * Settings.MasterVolume;
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