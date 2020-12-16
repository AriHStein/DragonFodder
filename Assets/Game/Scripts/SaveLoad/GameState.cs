using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObject/GameState", order = 82)]
public class GameState : ScriptableObject
{
    public GameSaveData Data;

    public void CopyState(GameState other)
    {
        Data = other.Data;
    }

    public void Clear()
    {
        Data = null;
    }

    public void LoadNewGameState()
    {
        GameState defaultState = Resources.Load("DefaultGameState", typeof(GameState)) as GameState;
        if(defaultState == null)
        {
            Debug.LogError($"Default state not found.");
            return;
        }

        CopyState(defaultState);
    }

    public void LoadQuickPlayState()
    {
        LoadNewGameState();
        List<RecruitableUnitCollection> recruitableUnits = Data.RecruitableUnits;
        foreach(RecruitableUnitCollection rUnit in recruitableUnits)
        {
            for (int i = 0; i < rUnit.Count; i++)
            { 
                Data.CurrentUnits.Add(new UnitData(rUnit.Proto, Faction.Player)); 
            }
        }

        Data.RecruitableUnits = new List<RecruitableUnitCollection>();
    }

    public void SaveGame()
    {
        SaveLoadUtility.SaveGame(Data);
        PrefKeys.SetContinueFileName(Data.fileName);
    }

    public void GameOver()
    {
        Data.GameOver = true;
        PrefKeys.ClearContinueFile();
        SaveLoadUtility.DeleteGame(Data.fileName);
    }
}
