using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public Vector2Int BoardSize;
    public SquadData Enemies;
    public int RowsAllowedForPlayerUnits;

    public int Reward;

    public bool Complete;

    public Encounter(SquadData enemies, Vector2Int boardSize, int playerRows, int reward)
    {
        Enemies = enemies;
        BoardSize = boardSize;
        RowsAllowedForPlayerUnits = playerRows;
        Reward = reward;
        Complete = false;
    }
}
