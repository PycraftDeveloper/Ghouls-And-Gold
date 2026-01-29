using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This program is used to store the game's configuration when the game is closed.

[System.Serializable]
public class GameData
{
    // This class stores a copy of registry entries to be saved/loaded from disk.
    public float Master_Volume = 1.0f;

    public float SFX_Volume = 1.0f;
    public float Music_Volume = 1.0f;
}

public class SavedDataManager
{
    // Used to save/load content from a save location - expand to save game progress.
    private string SaveFileLocation = Application.persistentDataPath;

    private string SaveFileName = "GameData.json";
    private string SavePath = "";

    public SavedDataManager()
    {
        SavePath = Path.Combine(SaveFileLocation, SaveFileName);
    }

    public void Load()
    {
        // Loads data from disk, or uses defaults if save not found.
        GameData LoadedData;
        if (File.Exists(SavePath))
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(SavePath, FileMode.Open)) // Open file
            {
                using (StreamReader reader = new StreamReader(stream)) // Open stream
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            LoadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            Registry.Master_Volume = LoadedData.Master_Volume;
            Registry.SFX_Volume = LoadedData.SFX_Volume;
            Registry.Music_Volume = LoadedData.Music_Volume;
        }
    }

    public void Save()
    {
        // Saves data to disk.
        Directory.CreateDirectory(SaveFileLocation);

        GameData SavedData = new GameData();

        SavedData.Master_Volume = Registry.Master_Volume;
        SavedData.SFX_Volume = Registry.SFX_Volume;
        SavedData.Music_Volume = Registry.Music_Volume;

        string SerialisedGameData = JsonUtility.ToJson(SavedData, true);

        using (FileStream stream = new FileStream(SavePath, FileMode.Create)) // Open file
        {
            using (StreamWriter writer = new StreamWriter(stream)) // Open stream
            {
                writer.Write(SerialisedGameData);
            }
        }
    }
}