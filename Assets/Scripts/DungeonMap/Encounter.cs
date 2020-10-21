using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public Vector2Int MapPosition;
    public List<Encounter> Connections;
    
    public Vector2Int BoardSize;
    public SquadData Enemies;
    public int RowsAllowedForPlayerUnits;

    public int Reward;

    public bool Complete;

    public Encounter(Vector2Int mapPosition, SquadData enemies, Vector2Int boardSize, int playerRows, int reward)
    {
        MapPosition = mapPosition;
        Enemies = enemies;
        BoardSize = boardSize;
        RowsAllowedForPlayerUnits = playerRows;
        Reward = reward;
        Complete = false;

        Connections = new List<Encounter>();
    }

    public void ConnectTo(Encounter other)
    {
        if(other == null || other == this)
        {
            Debug.LogError("Invalid connection.");
            return;
        }

        if(Connections.Contains(other))
        {
            Debug.LogWarning("Already connected to other encounter.");
            return;
        }

        Connections.Add(other);
    }
}
