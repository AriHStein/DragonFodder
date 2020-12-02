using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadUtility
{
    const string SAVE_DIR = "Saves";
    const string SAVE_EXTENSION = ".sav";
    
    public static void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        string dirPath = Path.Combine(Application.persistentDataPath, SAVE_DIR);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string filePath = Path.Combine(dirPath, data.fileName, SAVE_EXTENSION);
        File.WriteAllText(filePath, json);
    }

    public static GameSaveData LoadGame(string fileName)
    {
        string dirPath = Path.Combine(Application.persistentDataPath, SAVE_DIR);
        string filePath = Path.Combine(dirPath, fileName, SAVE_EXTENSION);

        if(!File.Exists(filePath))
        {
            Debug.LogError($"No file exsits at {filePath}.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        return data;
    }

    public static bool SaveFileExists(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_DIR, fileName, SAVE_EXTENSION);
        return File.Exists(path);
    }
}
