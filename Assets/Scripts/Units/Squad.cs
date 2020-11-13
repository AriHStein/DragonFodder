using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Squad
{
    public SquadData Data { get; protected set; }

    public Squad(SquadData data)
    {
        Data = data;
    }
    public void UpdateUnits(List<Unit> updatedUnits)
    {
        // iterate through all units in this squads unit list.
        // if the unit exists in the updated unit list, update that unit.
        // otherwise, assume the unit died during the battle and remove it from the squad.
        if (Data.Units == null || updatedUnits == null)
        {
            Data = new SquadData();
            return;
        }

        List<UnitSerializationData> newSquad = new List<UnitSerializationData>();
        foreach(Unit unit in updatedUnits)
        {
            if(unit.IsSummoned)
            {
                continue;
            }
            
            bool newUnit = true;
            foreach(UnitSerializationData existing in Data.Units)
            {                
                if(unit.ID == existing.ID)
                {
                    newUnit = false;
                    newSquad.Add(existing.UpdateUnitStatus(unit));
                    break;
                }
            }

            if (newUnit)
            {
                newSquad.Add(new UnitSerializationData(unit, unit.Square.Position));
            }
        }

        Data = new SquadData(newSquad, Data.SquadOrigin, Data.Faction);
    }
}

[System.Serializable]
public struct SquadData
{
    public List<UnitSerializationData> Units;
    public Vector2Int SquadOrigin;
    public Faction Faction;

    [System.NonSerialized]
    public Vector2Int Size;
    [System.NonSerialized]
    public int Difficulty;

    private SquadData(SquadData original)
    {
        Units = new List<UnitSerializationData>(original.Units);
        SquadOrigin = original.SquadOrigin;
        Faction = original.Faction;

        Size = Vector2Int.zero;
        Difficulty = 1;
        RecalculateParameters();
    }

    public SquadData(List<UnitSerializationData> units, Vector2Int origin, Faction faction = Faction.Player)
    {
        if (units != null)
        {
            Units = units;
        }
        else
        {
            Units = new List<UnitSerializationData>();
        }

        SquadOrigin = origin;
        Faction = faction;
        Size = Vector2Int.zero;
        Difficulty = 1;
        RecalculateParameters();
    }

    public SquadData Clone()
    {
        return new SquadData(this);
    }

    public static SquadData CombineSquads(List<SquadData> squads)
    {
        if(squads == null || squads.Count == 0)
        {
            return new SquadData();
        }

        if(squads.Count == 1)
        {
            return squads[0];
        }
        
        SquadData combinedSquad = new SquadData();
        combinedSquad.Faction = squads[0].Faction;
        Vector2Int origin = squads[0].SquadOrigin;
        foreach(SquadData squad in squads)
        {
            if(squad.Faction != combinedSquad.Faction)
            {
                Debug.LogError("Squad factions do not match.");
                return new SquadData();
            }

            if(squad.SquadOrigin.x < origin.x)
            {
                origin.x = squad.SquadOrigin.x;
            }

            if (squad.SquadOrigin.y < origin.y)
            {
                origin.y = squad.SquadOrigin.y;
            }
        }
        combinedSquad.SquadOrigin = origin;

        combinedSquad.Units = new List<UnitSerializationData>();
        foreach(SquadData squad in squads)
        {
            Vector2Int offset = squad.SquadOrigin - combinedSquad.SquadOrigin;
            foreach(UnitSerializationData unit in squad.Units)
            {
                UnitSerializationData clone = unit.Clone();
                clone.Position += offset;
                combinedSquad.Units.Add(clone);
            }
        }

        combinedSquad.UpdateSize();
        return combinedSquad;
    }

    public void RecalculateParameters()
    {
        UpdateSize();
        UpdateDifficulty();
    }

    private void UpdateSize()
    {
        if (Units == null || Units.Count == 0)
        {
            Size = Vector2Int.zero;
            return;
        }

        Vector2Int max = Vector2Int.zero;
        foreach (UnitSerializationData unit in Units)
        {
            if (unit.Position.x - SquadOrigin.x > max.x)
            {
                max.x = unit.Position.x - SquadOrigin.x;
            }

            if (unit.Position.y - SquadOrigin.y > max.y)
            {
                max.y = unit.Position.y - SquadOrigin.y;
            }
        }

        Size = max;
    }

    private void UpdateDifficulty()
    {
        Difficulty = 0;
        foreach(UnitSerializationData unit in Units)
        {
            Difficulty += unit.Difficulty;
        }
    }
}
