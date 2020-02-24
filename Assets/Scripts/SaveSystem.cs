using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSettings(SettingsData iData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string settingsPath = Application.persistentDataPath + "/settings.bin";
        FileStream settingsSaveStream = new FileStream(settingsPath, FileMode.Create);
        SettingsData data = new SettingsData(iData);
        formatter.Serialize(settingsSaveStream, data);
        settingsSaveStream.Close();
    }

    public static SettingsData LoadSettings()
    {
        string settingsPath = Application.persistentDataPath + "/settings.bin";

        if (File.Exists(settingsPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream settingsLoadStream = new FileStream(settingsPath, FileMode.Open);
            SettingsData data = formatter.Deserialize(settingsLoadStream) as SettingsData;
            settingsLoadStream.Close();
            return data;
        }
        else
        {
            // DEBUG
            #if UNITY_EDITOR
            Debug.LogWarning("Settings data file not found in " + settingsPath);
            #endif

            return null;
        }
    }

    public static void SavePlayers(PlayersData iData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string playersPath = Application.persistentDataPath + "/players.bin";
        FileStream playersSaveStream = new FileStream(playersPath, FileMode.Create);
        PlayersData data = new PlayersData(iData);
        formatter.Serialize(playersSaveStream, data);
        playersSaveStream.Close();
    }

    public static PlayersData LoadPlayers()
    {
        string playersPath = Application.persistentDataPath + "/players.bin";

        if (File.Exists(playersPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream playersLoadStream = new FileStream(playersPath, FileMode.Open);
            PlayersData data = formatter.Deserialize(playersLoadStream) as PlayersData;
            playersLoadStream.Close();
            return data;
        }
        else
        {
            // DEBUG
            #if UNITY_EDITOR
            Debug.LogWarning("Players data file not found in " + playersPath);
            #endif
            
            return null;
        }
    }
}