using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public Vector2Int BoardSize;
    public SquadData Enemies;

    public int Reward;

    public bool Complete;

    public Encounter(SquadData enemies, Vector2Int boardSize, int reward)
    {
        Enemies = enemies;
        BoardSize = boardSize;
        Reward = reward;
        Complete = false;
    }
}
