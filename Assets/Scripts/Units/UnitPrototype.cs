using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UnitData", menuName = "Units/UnitData", order = 100)]
[System.Serializable]
public class UnitPrototype : ScriptableObject
{
    //public Vector2Int Position;
    public string Type;
    public Faction Faction;
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

[System.Serializable]
public struct UnitData
{
    public string Type;
    public Vector2Int Position;
    public Faction Faction;
    public int MaxHealth;
    public int CurrentHealth;

    public UnitData(string type, Vector2Int position, Faction faction, int maxHealth, int currentHealth)
    {
        Type = type;
        Position = position;
        Faction = faction;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
    }

    public UnitData(Unit unit)
    {
        Type = unit.Type;
        Position = unit.Square.Position;
        Faction = unit.Faction;
        MaxHealth = unit.MaxHealth;
        CurrentHealth = unit.CurrentHealth;
    }

    public UnitData(UnitPrototype proto, Vector2Int position)
    {
        Type = proto.Type;
        Position = position;
        Faction = proto.Faction;
        MaxHealth = proto.MaxHealth;
        CurrentHealth = proto.MaxHealth;
    }

    public UnitData Clone()
    {
        return new UnitData(Type, Position, Faction, MaxHealth, CurrentHealth);
    }
}
