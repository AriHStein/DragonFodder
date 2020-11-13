using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct UnitSerializationData
{
    public string Type;
    public SerializableGuid ID;
    public Vector2Int Position;
    public Faction Faction;
    public int CurrentHealth;
    public int CurrentMP;
    public int Difficulty;
    public bool IsSummoned;

    private UnitSerializationData(
        string type,
        Guid id,
        int difficulty,
        Vector2Int position,
        Faction faction,
        int currentHealth,
        int currentMP,
        bool isSummon = false)
    {
        Type = type;
        ID = id;
        Difficulty = difficulty;
        Position = position;
        Faction = faction;
        CurrentHealth = currentHealth;
        CurrentMP = currentMP;
        IsSummoned = isSummon;
    }

    public UnitSerializationData(Unit unit, Vector2Int origin)
    {
        Type = unit.Type;
        ID = unit.ID;
        Difficulty = unit.Difficulty;
        Position = unit.Square.Position - origin;
        Faction = unit.Faction;
        CurrentHealth = unit.CurrentHealth;
        CurrentMP = unit.CurrentMP;
        IsSummoned = unit.IsSummoned;
    }

    public UnitSerializationData(UnitPrototype proto, Vector2Int position, Faction faction, bool isSummon = false)
    {
        Type = proto.Type;
        ID = Guid.NewGuid();
        Difficulty = proto.Difficulty;
        Position = position;
        Faction = faction;
        CurrentHealth = proto.MaxHealth;
        CurrentMP = 0;
        IsSummoned = isSummon;
    }

    public UnitSerializationData Clone()
    {
        return new UnitSerializationData(
            Type,
            ID,
            Difficulty,
            Position,
            Faction,
            CurrentHealth,
            CurrentMP,
            IsSummoned);
    }

    public UnitSerializationData UpdateUnitStatus(Unit unit)
    {
        if (unit.ID != ID)
        {
            Debug.LogWarning($"UpdateUnitStatus :: Unit IDs do not match. Old ID: {ID}, new ID: {unit.ID}.");
        }

        UnitSerializationData newUnit = Clone();
        newUnit.CurrentHealth = unit.CurrentHealth;
        newUnit.CurrentMP = unit.CurrentMP;
        return newUnit;
    }
}
