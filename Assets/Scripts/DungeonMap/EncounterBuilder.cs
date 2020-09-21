using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncounterBuilder
{
    static readonly Vector2Int MIN_BOARD_SIZE = new Vector2Int(6, 6);
    static readonly int MAX_BOARD_ROWS = 12;
    
    public static Encounter GenerateEncounter(int difficulty)
    {
        Encounter e = new Encounter();
        e.Enemies = SquadBuilder.GenerateFormationFromEnemySquads(difficulty);

        // Board should be at least 6x6. It should be wide enough to place the whole enemy squad.
        // It should be square, unless the enemy squad is larger than 12 wide, in which case it should only have 12 rows.
        e.BoardSize = MIN_BOARD_SIZE;
        e.BoardSize.x = Mathf.Max(e.BoardSize.x, e.Enemies.Size.x + 1);
        e.BoardSize.y = Mathf.Min(MAX_BOARD_ROWS, e.BoardSize.x);

        return e;
    }
}
