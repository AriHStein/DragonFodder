using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UnitSaveLoadUtility
{
    const string m_squadSaveFolder = "SquadData";
    const string m_squadFileExtension = ".json";

    public static void SaveSquad(SquadData squad, string fileName)
    {
        string json = JsonUtility.ToJson(squad, true);
        if(!Directory.Exists(SquadDirectory()))
        {
            Directory.CreateDirectory(SquadDirectory());
        }

        string filePath = EnemySquadPath(fileName);
        File.WriteAllText(filePath, json);
        //if(!File.Exists(filePath))
        //{
        //    File.Create(filePath);
        //}
        
        //File.AppendAllText(filePath, json);
        Debug.Log(filePath);
    }

    public static List<SquadData> LoadAllSquads()
    {
        List<SquadData> squads = new List<SquadData>();
        string[] files = GetSquadFiles();
        if(files == null || files.Length == 0)
        {
            Debug.LogError($"SquadData files not found.");
            return null;
        }

        foreach(string file in files)
        {
            squads.Add(LoadSquad(file));
        }

        return squads;
    } 

    private static SquadData LoadSquad(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"No SquadData file exists at {path}.");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SquadData>(json);
    }

    private static string SquadDirectory()
    {
        return Path.Combine(Application.dataPath, m_squadSaveFolder);
    }

    private static string EnemySquadPath(string fileName)
    {
        return Path.Combine(SquadDirectory(), fileName + m_squadFileExtension);
    }

    private static string[] GetSquadFiles()
    {
        return Directory.GetFiles(SquadDirectory(), "*" + m_squadFileExtension);
    }
}
