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
    public int AttackDamage;
    public float TimeBetweenActions;

    public int Difficulty = 1;
    public bool Flying = false;

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
    //public UnitPrototype Proto;
    public string Type;
    public SerializableGuid ID;
    public Vector2Int Position;
    public Faction Faction;
    //public int MaxHealth;
    public int CurrentHealth;
    public int Difficulty;

    private UnitData(
        string type, 
        //UnitPrototype proto,
        Guid id, 
        int difficulty,
        Vector2Int position, 
        Faction faction, 
        //int maxHealth, 
        int currentHealth)
    {
        Type = type;
        //Proto = proto;
        ID = id;
        Difficulty = difficulty;
        Position = position;
        Faction = faction;
        //MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    public UnitData(Unit unit, Vector2Int origin)
    {
        Type = unit.Proto.Type;
        //Proto = unit.Proto;
        ID = unit.ID;
        Difficulty = unit.Proto.Difficulty;
        Position = unit.Square.Position - origin;
        Faction = unit.Faction;
        //MaxHealth = unit.MaxHealth;
        CurrentHealth = unit.CurrentHealth;
    }

    public UnitData(UnitPrototype proto, Vector2Int position, Faction faction)
    {
        Type = proto.Type;
        //Proto = proto;
        ID = Guid.NewGuid();
        Difficulty = proto.Difficulty;
        Position = position;
        Faction = faction;
        //MaxHealth = proto.MaxHealth;
        CurrentHealth = proto.MaxHealth;
    }

    public UnitData Clone()
    {
        return new UnitData(
            Type, 
            //Proto,
            ID, 
            Difficulty,
            Position, 
            Faction, 
            //MaxHealth, 
            CurrentHealth);
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
