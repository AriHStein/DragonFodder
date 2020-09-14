using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "new Squad", menuName = "Units/SquadData", order = 101)]
[System.Serializable]
public struct SquadData
{
    //public static SquadData GetNewSquad(List<UnitData> units, Vector2Int origin)
    //{
    //    SquadData newSquad = ScriptableObject.CreateInstance<SquadData>();
    //    if(units != null)
    //    {
    //        newSquad.Units = units;
    //    } else
    //    {
    //        newSquad.Units = new List<UnitData>();
    //    }

    //    newSquad.SquadOrigin = origin;
    //    newSquad.UpdateSize();
    //    return newSquad;
    //}

    public List<UnitData> Units;
    public Vector2Int SquadOrigin;
    public Faction Faction;

    [System.NonSerialized]
    public Vector2Int Size;

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

    public void UpdateSize()
    {
        if(Units == null || Units.Count == 0)
        {
            Size = Vector2Int.zero;
            return;
        }

        Vector2Int max = Vector2Int.zero;
        foreach(UnitData unit in Units)
        {
            if(unit.Position.x > max.x)
            {
                max.x = unit.Position.x;
            }

            if(unit.Position.y > max.y)
            {
                max.y = unit.Position.y;
            }
        }

        Size = max - SquadOrigin;
    }
}
