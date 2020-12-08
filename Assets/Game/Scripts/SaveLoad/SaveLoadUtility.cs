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

        string filePath = Path.Combine(dirPath, data.fileName + SAVE_EXTENSION);
        File.WriteAllText(filePath, json);
    }

    public static GameSaveData LoadGame(string fileName)
    {
        if(fileName == null)
        {
            Debug.LogError("File name is null.");
            return null;
        }
        
        string dirPath = Path.Combine(Application.persistentDataPath, SAVE_DIR);
        string filePath = Path.Combine(dirPath, fileName + SAVE_EXTENSION);

        if(!File.Exists(filePath))
        {
            Debug.LogError($"No file exsits at {filePath}.");
            return null;
        }

        string json = File.ReadAllText(filePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        return data;
    }

    public static void DeleteGame(string fileName)
    {
        if (fileName == null)
        {
            Debug.LogError("File name is null.");
            return;
        }

        string dirPath = Path.Combine(Application.persistentDataPath, SAVE_DIR);
        string filePath = Path.Combine(dirPath, fileName + SAVE_EXTENSION);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"No file exsits at {filePath}.");
            return;
        }

        File.Delete(filePath);
    }

    public static bool SaveFileExists(string fileName)
    {
        if(fileName == null)
        {
            return false;
        }
        
        string path = Path.Combine(Application.persistentDataPath, SAVE_DIR, fileName + SAVE_EXTENSION);
        return File.Exists(path);
    }

    public static List<string> GetSaveFileNames()
    {
        List<string> saves = new List<string>();
        string dirPath = Path.Combine(Application.persistentDataPath, SAVE_DIR);
        if(!Directory.Exists(dirPath))
        {
            return saves;
        }

        string[] files = Directory.GetFiles(dirPath, "*" + SAVE_EXTENSION);
        foreach(string file in files)
        {
            saves.Add(Path.GetFileNameWithoutExtension(file));
        }

        return saves;
    }
}
