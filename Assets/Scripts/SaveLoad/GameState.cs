using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
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

    public void LoadDefaultState()
    {
        GameState defaultState = Resources.Load("DefaultGameState", typeof(GameState)) as GameState;
        if(defaultState == null)
        {
            Debug.LogError($"Default state not found.");
            return;
        }

        CopyState(defaultState);
    }
}
