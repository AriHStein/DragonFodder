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
    public void UpdateStatuses(List<Unit> updatedUnits)
    {
        // iterate through all units in this squads unit list.
        // if the unit exists in the updated unit list, update that unit.
        // otherwise, assume the unit died during the battle and remove it from the squad.

        // TODO: Do we need to account for units that exist in the new units list, but not in the original squad?
        // Theese might be summons or other temporary units. Assuming they die after the battle seems reasonable for now.
        Debug.Log($"Existing count: {Data.Units.Count}");
        Debug.Log($"New unit count: {updatedUnits.Count}");


        if (Data.Units == null || updatedUnits == null)
        {
            Data = new SquadData();
            return;
        }

        List<UnitData> newSquad = new List<UnitData>();
        foreach(Unit unit in updatedUnits)
        {
            bool newUnit = true;
            foreach(UnitData existing in Data.Units)
            {
                if(unit.ID == existing.ID)
                {
                    newUnit = false;
                    newSquad.Add(existing.UpdateUnitStatus(unit));
                }
            }

            if (newUnit)
            {
                Data.Units.Add(new UnitData(unit, unit.Square.Position));
            }
        }

        //for (int i = Data.Units.Count - 1; i >= 0; i--)
        //{
        //    bool unitFound = false;
        //    foreach (Unit unit in updatedUnits)
        //    {
        //        if (unit.ID != Guid.Empty && unit.ID == Data.Units[i].ID)
        //        {
        //            unitFound = true;
        //            Data.Units[i] = Data.Units[i].UpdateUnitStatus(unit);
        //        }
        //    }

        //    if (!unitFound)
        //    {
        //        Data.Units.RemoveAt(i);
        //    }
        //}

        //foreach(Unit unit in updatedUnits)
        //{
        //    bool newUnit = true;
        //    foreach(UnitData data in Data.Units)
        //    {
        //        if(unit.ID == data.ID)
        //        {
        //            newUnit = false;
        //            break;
        //        }
        //    }

        //    if(newUnit)
        //    {
        //        Data.Units.Add(new UnitData(unit, unit.Square.Position));
        //    }
        //}

        //Data = new SquadData(Data.Units, Data.SquadOrigin, Data.Faction);
        Data = new SquadData(newSquad, Data.SquadOrigin, Data.Faction);
    }
}

//[CreateAssetMenu(fileName = "new Squad", menuName = "Units/SquadData", order = 101)]
[System.Serializable]
public struct SquadData
{
    public List<UnitData> Units;
    public Vector2Int SquadOrigin;
    public Faction Faction;

    [System.NonSerialized]
    public Vector2Int Size;
    [System.NonSerialized]
    public int Difficulty;

    private SquadData(SquadData original)
    {
        Units = new List<UnitData>(original.Units);
        SquadOrigin = original.SquadOrigin;
        Faction = original.Faction;

        Size = Vector2Int.zero;
        Difficulty = 1;
        RecalculateParameters();
    }

    public SquadData(List<UnitData> units, Vector2Int origin, Faction faction = Faction.Player)
    {
        if (units != null)
        {
            Units = units;
        }
        else
        {
            Units = new List<UnitData>();
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

        combinedSquad.Units = new List<UnitData>();
        foreach(SquadData squad in squads)
        {
            Vector2Int offset = squad.SquadOrigin - combinedSquad.SquadOrigin;
            foreach(UnitData unit in squad.Units)
            {
                UnitData clone = unit.Clone();
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
        foreach (UnitData unit in Units)
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
        foreach(UnitData unit in Units)
        {
            Difficulty += unit.Difficulty;
        }
    }
}
