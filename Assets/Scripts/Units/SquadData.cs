using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "new Squad", menuName = "Units/SquadData", order = 101)]
[System.Serializable]
public struct SquadData
{
    public List<UnitData> Units;
    public Vector2Int SquadOrigin;
    public Faction Faction;

    [System.NonSerialized]
    public Vector2Int Size;

    private SquadData(SquadData original)
    {
        Units = new List<UnitData>(original.Units);
        SquadOrigin = original.SquadOrigin;
        Faction = original.Faction;

        Size = Vector2Int.zero;
        UpdateSize();
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
        UpdateSize();
    }

    public SquadData Clone()
    {
        return new SquadData(this);
    }

    public static SquadData CombineSquads(List<SquadData> squads)
    {
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

    public void UpdateSize()
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

    public SquadData UpdateUnitStatuses(List<Unit> updatedUnits)
    {
        // iterate through all units in this squads unit list.
        // if the unit exists in the updated unit list, update that unit.
        // otherwise, assume the unit died during the battle and remove it from the squad.

        // TODO: Do we need to account for units that exist in the new units list, but not in the original squad?
        // Theese might be summons or other temporary units. Assuming they die after the battle seems reasonable for now.
        
        for(int i = Units.Count - 1; i >= 0; i--)
        {
            bool unitFound = false;
            foreach(Unit unit in updatedUnits)
            {
                if(unit.ID == Units[i].ID)
                {
                    unitFound = true;
                    Units[i] = Units[i].UpdateUnitStatus(unit);
                }
            }

            if(!unitFound)
            {
                Units.RemoveAt(i);
            }
        }

        return new SquadData(Units, SquadOrigin, Faction);
    }
}
