﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSettings(SettingsData iData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string settingsPath = Application.persistentDataPath + "/settings.bin";
        // File mode is create because we want to overwrite existing files
        FileStream stream = new FileStream(settingsPath, FileMode.Create);        
        SettingsData data = new SettingsData(iData);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SettingsData LoadSettings()
    {
        string settingsPath = Application.persistentDataPath + "/settings.bin";

        if (File.Exists(settingsPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(settingsPath, FileMode.Open);
            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Settings data file not found in " + settingsPath);
            return null;
        }
    } 
}