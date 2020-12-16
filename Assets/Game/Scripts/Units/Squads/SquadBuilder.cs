using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquadBuilder
{
    //static List<SquadData> m_enemySquadProtos;
    //static List<SquadData> m_bossSquadProtos;
    
    public static Squad GenerateFormationFromEnemySquads(int difficulty)
    {
        //if(m_enemySquadProtos == null || m_enemySquadProtos.Count == 0)
        //{
        //    LoadEnemySquads();
        //}

        //return GenerateFormation(m_enemySquadProtos, difficulty);
        return GenerateFormation(SquadPrototypeLookup.GetPrototypes((proto) => { return proto.Faction == Faction.Enemy && !proto.IsBoss; }), difficulty);
    }

    //static void LoadEnemySquads()
    //{
    //    m_enemySquadProtos = UnitSaveLoadUtility.LoadAllSquadsInDir("Enemy");
    //    m_enemySquadProtos.Sort((x, y) => x.Difficulty.CompareTo(y.Difficulty));
    //}


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
            //Squad newSquad = squadProtos[Random.Range(0, squadProtos.Count)].Clone();
            Squad newSquad = new Squad(squadProtos[Random.Range(0, squadProtos.Count)], offset);

            if (totalDifficulty + newSquad.Difficulty > difficulty)
            {
                // Use the easiest squad if a randomly chosen squad overshoots the difficulty threshold
                //Debug.Log("Using minimum difficulty squad.");
                newSquad = new Squad(squadProtos[0], offset);
            }
            //newSquad.RecalculateParameters();
            newSquad.SquadOrigin = newSquad.SquadOrigin + offset;
            squadsToCombine.Add(newSquad);
            offset.x += newSquad.Size.x + 1;
            totalDifficulty += newSquad.Difficulty;
        }
        
        //for (int i = 0; i < difficulty; i++)
        //{
        //    SquadData newSquad = squadProtos[Random.Range(0, squadProtos.Count)].Clone();
        //    newSquad.SquadOrigin = newSquad.SquadOrigin + offset;
        //    squadsToCombine.Add(newSquad);
        //    offset.x += newSquad.Size.x + 1;
        //}

        return Squad.CombineSquads(squadsToCombine);
    }


    //static void LoadBossSquads()
    //{
    //    m_bossSquadProtos = UnitSaveLoadUtility.LoadAllSquadsInDir("Boss");
    //}

    public static Squad GenerateBossFormation(Vector2Int boardSize)
    {
        //if (m_bossSquadProtos == null || m_bossSquadProtos.Count == 0)
        //{
        //    LoadBossSquads();
        //}

        List<SquadPrototype> bossSquads = SquadPrototypeLookup.GetPrototypes((proto) => { return proto.IsBoss; });
        if (bossSquads == null || bossSquads.Count == 0)
        {
            Debug.Log($"No boss protos found.");
            return new Squad();
        }

        //Squad newSquad = bossSquads[Random.Range(0, bossSquads.Count)].Clone();
        Squad newSquad = new Squad(bossSquads[Random.Range(0, bossSquads.Count)], Vector2Int.zero);

        //newSquad.RecalculateParameters();

        Vector2Int offset = Vector2Int.zero;
        offset.x += boardSize.x / 2;
        offset.x -= newSquad.Size.x / 2;
        newSquad.SquadOrigin = newSquad.SquadOrigin + offset;

        return newSquad;
    }
}
