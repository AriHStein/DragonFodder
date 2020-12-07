using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquadBuilder
{
    static List<SquadData> m_enemySquadProtos;
    static List<SquadData> m_bossSquadProtos;
    
    public static SquadData GenerateFormationFromEnemySquads(int difficulty)
    {
        if(m_enemySquadProtos == null || m_enemySquadProtos.Count == 0)
        {
            LoadEnemySquads();
        }
        
        return GenerateFormation(m_enemySquadProtos, difficulty);
    }

    static void LoadEnemySquads()
    {
        m_enemySquadProtos = UnitSaveLoadUtility.LoadAllSquadsInDir("Enemy");
        m_enemySquadProtos.Sort((x, y) => x.Difficulty.CompareTo(y.Difficulty));
    }


    public static SquadData GenerateFormation(List<SquadData> squadProtos, int difficulty)
    {
        if(squadProtos == null || squadProtos.Count == 0 || difficulty < squadProtos[0].Difficulty)
        {
            Debug.Log($"No squad protos. Min difficulty: {squadProtos[0].Difficulty}. Attempted difficulty: {difficulty}");
            return new SquadData();
        }
        
        List<SquadData> squadsToCombine = new List<SquadData>();
        Vector2Int offset = Vector2Int.zero;
        int totalDifficulty = 0;
        while(totalDifficulty < difficulty)
        {
            SquadData newSquad = squadProtos[Random.Range(0, squadProtos.Count)].Clone();
            if(totalDifficulty + newSquad.Difficulty > difficulty)
            {
                // Use the easiest squad if a randomly chosen squad overshoots the difficulty threshold
                //Debug.Log("Using minimum difficulty squad.");
                newSquad = squadProtos[0].Clone();
            }
            newSquad.RecalculateParameters();
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

        return SquadData.CombineSquads(squadsToCombine);
    }


    static void LoadBossSquads()
    {
        m_bossSquadProtos = UnitSaveLoadUtility.LoadAllSquadsInDir("Boss");
    }

    public static SquadData GenerateBossFormation(Vector2Int boardSize)
    {
        if (m_bossSquadProtos == null || m_bossSquadProtos.Count == 0)
        {
            LoadBossSquads();
        }

        if (m_bossSquadProtos == null || m_bossSquadProtos.Count == 0)
        {
            Debug.Log($"No boss protos found.");
            return new SquadData();
        }

        SquadData newSquad = m_bossSquadProtos[Random.Range(0, m_bossSquadProtos.Count)].Clone();
        newSquad.RecalculateParameters();

        Vector2Int offset = Vector2Int.zero;
        offset.x += boardSize.x / 2;
        offset.x -= newSquad.Size.x / 2;
        newSquad.SquadOrigin = newSquad.SquadOrigin + offset;

        return newSquad;
    }
}
