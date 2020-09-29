using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquadBuilder
{
    static List<SquadData> m_enemySquadProtos;
    
    public static SquadData GenerateFormationFromEnemySquads(int size)
    {
        if(m_enemySquadProtos == null || m_enemySquadProtos.Count == 0)
        {
            LoadEnemySquads();
        }
        
        return GenerateFormation(m_enemySquadProtos, size);
    }

    static void LoadEnemySquads()
    {
        m_enemySquadProtos = UnitSaveLoadUtility.LoadAllSquadsInDir("Enemy");
    }

    public static SquadData GenerateFormation(List<SquadData> squadProtos, int numberSquads)
    {
        if(squadProtos == null || squadProtos.Count == 0)
        {
            return new SquadData();
        }
        
        List<SquadData> squadsToCombine = new List<SquadData>();
        Vector2Int offset = Vector2Int.zero;
        for (int i = 0; i < numberSquads; i++)
        {
            SquadData newSquad = squadProtos[Random.Range(0, squadProtos.Count)].Clone();
            newSquad.SquadOrigin = newSquad.SquadOrigin + offset;
            squadsToCombine.Add(newSquad);
            offset.x += newSquad.Size.x + 1;
        }

        return SquadData.CombineSquads(squadsToCombine);
    }
}
