using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Squad
{
    public List<UnitPositionPair> Units;
    public Vector2Int SquadOrigin;
    public Faction Faction;


    [Serializable]
    public struct UnitPositionPair
    {
        public UnitData Unit;
        public Vector2Int Position;

        public UnitPositionPair(UnitData unit, Vector2Int pos)
        {
            Unit = unit;
            Position = pos;
        }
    }

    [System.NonSerialized]
    public Vector2Int Size;
    [System.NonSerialized]
    public int Difficulty;

    public Squad()
    {
        Units = new List<UnitPositionPair>();
    }

    public Squad(
        //Dictionary<UnitData, Vector2Int> units, 
        List<Unit> units,
        Vector2Int origin,
        Faction faction = Faction.Player)
    {
        Units = new List<UnitPositionPair>();
        if (units != null)
        {
            foreach(Unit unit in units)
            {
                Units.Add(new UnitPositionPair(new UnitData(unit), unit.Square.Position - origin));
            }
            //Units = units;
        }
        //else
        //{
        //    //Units = new Dictionary<UnitData, Vector2Int>();
        //    Units = new List<UnitPositionPair>();
        //}

        SquadOrigin = origin;
        Faction = faction;
        Size = Vector2Int.zero;
        Difficulty = 1;
        RecalculateParameters();
    }

    public Squad(SquadPrototype proto, Vector2Int offset)
    {
        Faction = proto.Faction;
        SquadOrigin = offset;
        Units = new List<UnitPositionPair>();

        if(proto.Units != null)
        {
            for (int x = 0; x < proto.Units.Length; x++)
            {
                for (int y = 0; y < proto.Units[0].Length; y++)
                {
                    if (proto.Units[x] != null && proto.Units[x][y] != null)
                    {
                        Units.Add(new UnitPositionPair(new UnitData(proto.Units[x][y], Faction), new Vector2Int(x, y)));
                    }
                }
            }
        }

        RecalculateParameters();
    }

    private void RecalculateParameters()
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
        foreach (UnitPositionPair pair in Units)
        {
            if (pair.Position.x - SquadOrigin.x > max.x)
            {
                max.x = pair.Position.x - SquadOrigin.x;
            }

            if (pair.Position.y - SquadOrigin.y > max.y)
            {
                max.y = pair.Position.y - SquadOrigin.y;
            }
        }

        Size = max;
    }

    private void UpdateDifficulty()
    {
        Difficulty = 0;
        foreach (UnitPositionPair pair in Units)
        {
            Difficulty += pair.Unit.Difficulty;
        }
    }

    public static Squad CombineSquads(List<Squad> squads)
    {
        if (squads == null || squads.Count == 0)
        {
            return new Squad();
        }

        if (squads.Count == 1)
        {
            return squads[0];
        }

        Squad combinedSquad = new Squad();
        combinedSquad.Faction = squads[0].Faction;
        Vector2Int origin = squads[0].SquadOrigin;
        foreach (Squad squad in squads)
        {
            if (squad.Faction != combinedSquad.Faction)
            {
                Debug.LogError("Squad factions do not match.");
                return new Squad();
            }

            if (squad.SquadOrigin.x < origin.x)
            {
                origin.x = squad.SquadOrigin.x;
            }

            if (squad.SquadOrigin.y < origin.y)
            {
                origin.y = squad.SquadOrigin.y;
            }
        }
        combinedSquad.SquadOrigin = origin;
        combinedSquad.Units = new List<UnitPositionPair>();

        foreach (Squad squad in squads)
        {
            Vector2Int offset = squad.SquadOrigin - combinedSquad.SquadOrigin;

            foreach (UnitPositionPair pair in squad.Units)
            {
                UnitData clone = pair.Unit.Clone();
                Vector2Int position = pair.Position + offset;
                combinedSquad.Units.Add(new UnitPositionPair(clone, position));
            }
        }

        combinedSquad.UpdateSize();
        return combinedSquad;
    }
}

//[Serializable]
//public struct UnitPositionPair
//{
//    public UnitData Unit;
//    public Vector2Int Position;

//    public UnitPositionPair(UnitData unit, Vector2Int pos)
//    {
//        Unit = unit;
//        Position = pos;
//    }
//}

//[Serializable]
//public struct SquadData
//{
//    //public List<UnitData> Units;
//    //public Dictionary<UnitData, Vector2Int> Units;
//    public List<UnitPositionPair> Units;



//    public Vector2Int SquadOrigin;
//    public Faction Faction;

//    [System.NonSerialized]
//    public Vector2Int Size;
//    [System.NonSerialized]
//    public int Difficulty;

//    private SquadData(SquadData original)
//    {
//        //Units = new List<UnitData>(original.Units);
//        Units = new List<UnitPositionPair>(original.Units);
//        SquadOrigin = original.SquadOrigin;
//        Faction = original.Faction;

//        Size = Vector2Int.zero;
//        Difficulty = 1;
//        RecalculateParameters();
//    }

//    public SquadData(
//        //Dictionary<UnitData, Vector2Int> units, 
//        List<UnitPositionPair> units,
//        Vector2Int origin, 
//        Faction faction = Faction.Player)
//    {
//        if (units != null)
//        {
//            Units = units;
//        }
//        else
//        {
//            //Units = new Dictionary<UnitData, Vector2Int>();
//            Units = new List<UnitPositionPair>();
//        }

//        SquadOrigin = origin;
//        Faction = faction;
//        Size = Vector2Int.zero;
//        Difficulty = 1;
//        RecalculateParameters();
//    }

//    public SquadData Clone()
//    {
//        return new SquadData(this);
//    }

//    public static SquadData CombineSquads(List<SquadData> squads)
//    {
//        if(squads == null || squads.Count == 0)
//        {
//            return new SquadData();
//        }

//        if(squads.Count == 1)
//        {
//            return squads[0];
//        }
        
//        SquadData combinedSquad = new SquadData();
//        combinedSquad.Faction = squads[0].Faction;
//        Vector2Int origin = squads[0].SquadOrigin;
//        foreach(SquadData squad in squads)
//        {
//            if(squad.Faction != combinedSquad.Faction)
//            {
//                Debug.LogError("Squad factions do not match.");
//                return new SquadData();
//            }

//            if(squad.SquadOrigin.x < origin.x)
//            {
//                origin.x = squad.SquadOrigin.x;
//            }

//            if (squad.SquadOrigin.y < origin.y)
//            {
//                origin.y = squad.SquadOrigin.y;
//            }
//        }
//        combinedSquad.SquadOrigin = origin;

//        //combinedSquad.Units = new Dictionary<UnitData, Vector2Int>();
//        combinedSquad.Units = new List<UnitPositionPair>();

//        foreach (SquadData squad in squads)
//        {
//            Vector2Int offset = squad.SquadOrigin - combinedSquad.SquadOrigin;
//            //foreach(UnitData unit in squad.Units.Keys)
//            //{
//            //    UnitData clone = unit.Clone();
//            //    //clone.Position += offset;
//            //    Vector2Int position = squad.Units[unit] + offset;
//            //    combinedSquad.Units.Add(clone, position);
//            //}

//            foreach (UnitPositionPair pair in squad.Units)
//            {
//                UnitData clone = pair.Unit.Clone();
//                //clone.Position += offset;
//                Vector2Int position = pair.Position + offset;
//                combinedSquad.Units.Add(new UnitPositionPair(clone, position));
//            }
//        }

//        combinedSquad.UpdateSize();
//        return combinedSquad;
//    }

//    public void RecalculateParameters()
//    {
//        UpdateSize();
//        UpdateDifficulty();
//    }

//    private void UpdateSize()
//    {
//        if (Units == null || Units.Count == 0)
//        {
//            Size = Vector2Int.zero;
//            return;
//        }

//        Vector2Int max = Vector2Int.zero;
//        foreach (UnitPositionPair pair in Units)
//        {
//            if (pair.Position.x - SquadOrigin.x > max.x)
//            {
//                max.x = pair.Position.x - SquadOrigin.x;
//            }

//            if (pair.Position.y - SquadOrigin.y > max.y)
//            {
//                max.y = pair.Position.y - SquadOrigin.y;
//            }
//        }

//        Size = max;
//    }

//    private void UpdateDifficulty()
//    {
//        Difficulty = 0;
//        foreach(UnitPositionPair pair in Units)
//        {
//            Difficulty += pair.Unit.Difficulty;
//        }
//    }
//}
