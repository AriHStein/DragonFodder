using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonBuilder
{
    struct Path
    {
        public Vector2Int Start;
        public Vector2Int End;
        public Path(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }
    }

    public static void GenerateEncounters(int encounterCount, Vector2Int gridSize, out Dictionary<System.Guid, Encounter> encounters, out List<Encounter> availableEncoutners)
    {
        List<Vector2Int> weightedDirections = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.up,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.left,
            Vector2Int.down
        };


        List<Vector2Int> encounterPositions = new List<Vector2Int> { new Vector2Int(gridSize.x / 2, 0) };
        HashSet<Path> encouterPaths = new HashSet<Path>();

        int failedCount = 0;
        while (encounterPositions.Count < encounterCount && failedCount < 100)
        {
            Vector2Int start = encounterPositions[Random.Range(0, encounterPositions.Count)];
            Vector2Int end = start + weightedDirections[Random.Range(0, weightedDirections.Count)];
            if (end.x < 0 || end.y < 0 ||
                end.x >= gridSize.x || end.y >= gridSize.y)
            {
                failedCount++;
                continue;
            }

            encouterPaths.Add(new Path(start, end));
            if (!encounterPositions.Contains(end))
            {
                encounterPositions.Add(end);
            }
        }

        Encounter[,] encounterMap = new Encounter[gridSize.x, gridSize.y];
        encounters = new Dictionary<System.Guid, Encounter>();
        availableEncoutners = new List<Encounter>();

        int encounterDifficulty = 2;
        Encounter firstEncounter = null;
        for (int i = 0; i < encounterPositions.Count; i++)
        {

            Encounter e;
            if (i == encounterPositions.Count - 1)
            {
                e = EncounterBuilder.GenerateBossEncounter(encounterPositions[i]);
            }
            else
            {
                e = EncounterBuilder.GenerateEncounter(encounterPositions[i], encounterDifficulty);
            }
            encounterDifficulty++;

            encounterMap[encounterPositions[i].x, encounterPositions[i].y] = e;
            encounters[e.ID] = e;

            if (firstEncounter == null)
            {
                firstEncounter = e;
            }
        }

        foreach (Path path in encouterPaths)
        {
            encounterMap[path.Start.x, path.Start.y].ConnectTo(encounterMap[path.End.x, path.End.y]);
        }

        availableEncoutners.Add(firstEncounter);
        firstEncounter.Available = true;
    }
}
