using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UnitSaveLoadUtility
{
    const string m_squadSaveFolder = "SquadData";
    const string m_squadFileExtension = ".json";

    public static void SaveSquad(SquadData squad, string fileName, string dir)
    {
        string json = JsonUtility.ToJson(squad, true);
        if(!Directory.Exists(SquadDirectory()))
        {
            Directory.CreateDirectory(SquadDirectory());
        }

        string filePath = GetFilePath(fileName, dir);
        File.WriteAllText(filePath, json);
        //if(!File.Exists(filePath))
        //{
        //    File.Create(filePath);
        //}
        
        //File.AppendAllText(filePath, json);
        //Debug.Log(filePath);
    }

    public static List<SquadData> LoadAllSquadsInDir(string dirName)
    {
        List<SquadData> squads = new List<SquadData>();
        string[] files = GetSquadFilesInDir(dirName);
        if(files == null || files.Length == 0)
        {
            Debug.LogError($"SquadData files not found.");
            return null;
        }

        foreach(string file in files)
        {
            squads.Add(LoadSquadAtPath(file));
        }

        return squads;
    } 

    public static SquadData LoadSquad(string fileName, string directory)
    {
        return LoadSquadAtPath(GetFilePath(fileName, directory));
    }

    private static SquadData LoadSquadAtPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"No SquadData file exists at {path}.");
            return new SquadData();
        }

        string json = File.ReadAllText(path);
        SquadData squad = JsonUtility.FromJson<SquadData>(json);
        squad.UpdateSize();
        return squad;
    }

    private static string SquadDirectory()
    {
        return Path.Combine(Application.dataPath, m_squadSaveFolder);
    }

    private static string GetFilePath(string fileName, string directory)
    {
        return Path.Combine(Path.Combine(SquadDirectory(), directory), fileName + m_squadFileExtension);
    }

    private static string[] GetSquadFilesInDir(string dir)
    {
        return Directory.GetFiles(Path.Combine(SquadDirectory(), dir), "*" + m_squadFileExtension);
    }
}
