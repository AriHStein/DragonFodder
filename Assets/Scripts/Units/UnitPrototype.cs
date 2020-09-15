using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new UnitData", menuName = "Units/UnitData", order = 100)]
[System.Serializable]
public class UnitPrototype : ScriptableObject
{
    //public Vector2Int Position;
    public string Type;
    //public Faction Faction;
    public int MaxHealth;

    //public static UnitData GetData(Unit unit)
    //{
    //    //UnitData data = ScriptableObject.CreateInstance<UnitPrototype>();
    //    //data.Position = unit.Square.Position;
    //    //data.Type = unit.Type;
    //    //data.Faction = unit.Faction;

    //    //data.MaxHealth = unit.MaxHealth;
    //    //data.CurrentHealth = unit.CurrentHealth;

    //    return new UnitData(unit.Type, unit.Square.Position, unit.Faction, unit.MaxHealth, unit.CurrentHealth);
    //}

    //public UnitPrototype Clone()
    //{
    //    UnitPrototype newUnit = UnitPrototype.CreateInstance<UnitPrototype>();
    //    //newUnit.Position = Position;
    //    newUnit.Type = Type;
    //    newUnit.Faction = Faction;
    //    newUnit.MaxHealth = MaxHealth;
    //    newUnit.CurrentHealth = CurrentHealth;

    //    return newUnit;
    //}
}

[Serializable]
public struct UnitData
{
    public string Type;
    public SerializableGuid ID;
    public Vector2Int Position;
    public Faction Faction;
    public int MaxHealth;
    public int CurrentHealth;

    private UnitData(string type, Guid id, Vector2Int position, Faction faction, int maxHealth, int currentHealth)
    {
        Type = type;
        ID = id;
        Position = position;
        Faction = faction;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    public UnitData(Unit unit, Vector2Int origin)
    {
        Type = unit.Type;
        ID = unit.ID;
        Position = unit.Square.Position - origin;
        Faction = unit.Faction;
        MaxHealth = unit.MaxHealth;
        CurrentHealth = unit.CurrentHealth;
    }

    public UnitData(UnitPrototype proto, Vector2Int position, Faction faction)
    {
        Type = proto.Type;
        ID = Guid.NewGuid();
        Position = position;
        Faction = faction;
        MaxHealth = proto.MaxHealth;
        CurrentHealth = proto.MaxHealth;
    }

    public UnitData Clone()
    {
        return new UnitData(Type, ID, Position, Faction, MaxHealth, CurrentHealth);
    }

    public UnitData UpdateUnitStatus(Unit unit)
    {
        if(unit.ID != ID)
        {
            Debug.LogWarning($"UpdateUnitStatus :: Unit IDs do not match. Old ID: {ID}, new ID: {unit.ID}.");
        }
        
        //CurrentHealth = unit.CurrentHealth;

        UnitData newUnit = Clone();
        newUnit.CurrentHealth = unit.CurrentHealth;
        return newUnit;
    }
}
