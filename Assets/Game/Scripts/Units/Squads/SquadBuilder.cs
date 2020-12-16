using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquadBuilder
{
    public static Squad GenerateFormationFromEnemySquads(int difficulty)
    {
        return GenerateFormation(SquadPrototypeLookup.GetPrototypes((proto) => { return proto.Faction == Faction.Enemy && !proto.IsBoss; }), difficulty);
    }

    public static Squad GenerateFormation(List<SquadPrototype> squadProtos, int difficulty)
    {
        if(squadProtos == null || squadProtos.Count == 0 || difficulty < squadProtos[0].Difficulty)
        {
            Debug.Log($"No squad protos. Min difficulty: {squadProtos[0].Difficulty}. Attempted difficulty: {difficulty}");
            return new Squad();
        }

        squadProtos.Sort((x, y) => x.Difficulty.CompareTo(y.Difficulty));
        List<Squad> squadsToCombine = new List<Squad>();
        Vector2Int offset = Vector2Int.zero;
        int totalDifficulty = 0;
        while(totalDifficulty < difficulty)
        {
            Squad newSquad = new Squad(squadProtos[Random.Range(0, squadProtos.Count)], offset);

            if (totalDifficulty + newSquad.Difficulty > difficulty)
            {
                // Use the easiest squad if a randomly chosen squad overshoots the difficulty threshold
                //Debug.Log("Using minimum difficulty squad.");
                newSquad = new Squad(squadProtos[0], offset);
            }

            newSquad.SquadOrigin = newSquad.SquadOrigin + offset;
            squadsToCombine.Add(newSquad);
            offset.x += newSquad.Size.x + 1;
            totalDifficulty += newSquad.Difficulty;
        }

        return Squad.CombineSquads(squadsToCombine);
    }

    public static Squad GenerateBossFormation(Vector2Int boardSize)
    {
        List<SquadPrototype> bossSquads = SquadPrototypeLookup.GetPrototypes((proto) => { return proto.IsBoss; });
        if (bossSquads == null || bossSquads.Count == 0)
        {
            Debug.Log($"No boss protos found.");
            return new Squad();
        }

        Squad newSquad = new Squad(bossSquads[Random.Range(0, bossSquads.Count)], Vector2Int.zero);

        Vector2Int offset = Vector2Int.zero;
        offset.x += boardSize.x / 2;
        offset.x -= newSquad.Size.x / 2;
        newSquad.SquadOrigin = newSquad.SquadOrigin + offset;

        return newSquad;
    }
}
