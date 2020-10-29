using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new Unit Prototype", menuName = "Units/UnitPrototype", order = 111)]
[System.Serializable]
public class UnitPrototype : ScriptableObject
{
    public string Type;
    public int MaxHealth;
    public int MaxMP;
    //public int SpellMP;
    //public int AttackDamage;
    //public float AttackRange = 1f;
    public float TimeBetweenActions;

    public int Difficulty = 1;
    public bool Flying = false;
    public float MoveSpeed = 1f;

    public List<Ability> Abilities = default;
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
    public int CurrentMP;
    public int Difficulty;

    private UnitData(
        string type, 
        //UnitPrototype proto,
        Guid id, 
        int difficulty,
        Vector2Int position, 
        Faction faction, 
        //int maxHealth, 
        int currentHealth, 
        int currentMP)
    {
        Type = type;
        //Proto = proto;
        ID = id;
        Difficulty = difficulty;
        Position = position;
        Faction = faction;
        //MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        CurrentMP = currentMP;
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
        CurrentMP = unit.CurrentMP;
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
        CurrentMP = 0;
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
            CurrentHealth,
            CurrentMP);
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
        newUnit.CurrentMP = unit.CurrentMP;
        return newUnit;
    }
}
