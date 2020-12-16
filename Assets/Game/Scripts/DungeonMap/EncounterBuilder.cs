﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncounterBuilder
{
    static readonly Vector2Int MIN_BOARD_SIZE = new Vector2Int(6, 6);
    static readonly int MAX_BOARD_ROWS = 12;
    static readonly int MIN_PLAYER_ROWS = 2;
    static readonly int MAX_PLAYER_ROWS = 5;

    static readonly Vector2Int BOSS_BOARD_SIZE = new Vector2Int(20, 20);
    
    public static Encounter GenerateEncounter(Vector2Int mapPosition, int difficulty)
    {
        Squad enemies = SquadBuilder.GenerateFormationFromEnemySquads(difficulty);

        // Board should be at least 6x6. It should be wide enough to place the whole enemy squad.
        // It should be square, unless the enemy squad is larger than 12 wide, in which case it should only have 12 rows.
        Vector2Int boardSize = MIN_BOARD_SIZE;
        boardSize.x = Mathf.Max(boardSize.x, enemies.Size.x + 1);
        boardSize.y = Mathf.Min(MAX_BOARD_ROWS, boardSize.x);

        int playerRows = Mathf.Clamp(boardSize.y / 3, MIN_PLAYER_ROWS, MAX_PLAYER_ROWS);

        return new Encounter(mapPosition, enemies, boardSize, playerRows, difficulty);
    }

    public static Encounter GenerateBossEncounter(Vector2Int mapPosition)
    {
        // Board should be at least 6x6. It should be wide enough to place the whole enemy squad.
        // It should be square, unless the enemy squad is larger than 12 wide, in which case it should only have 12 rows.
        Vector2Int boardSize = BOSS_BOARD_SIZE;
        int playerRows = Mathf.Clamp(boardSize.y / 3, MIN_PLAYER_ROWS, MAX_PLAYER_ROWS);

        Squad enemies = SquadBuilder.GenerateBossFormation(BOSS_BOARD_SIZE);

        return new Encounter(mapPosition, enemies, boardSize, playerRows, enemies.Difficulty);
    }
}
