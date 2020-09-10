using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UnitData", menuName = "Units/UnitData", order = 100)]
public class UnitData : ScriptableObject
{
    public Vector2Int Position;
    public string Type;
    public Faction Faction;
    public int MaxHealth;
    public int CurrentHealth;

    public static UnitData GetData(Unit unit)
    {
        UnitData data = ScriptableObject.CreateInstance<UnitData>();
        data.Position = unit.Square.Position;
        data.Type = unit.Type;
        data.Faction = unit.Faction;

        data.MaxHealth = unit.MaxHealth;
        data.CurrentHealth = unit.CurrentHealth;

        return data;
    }

    public UnitData Clone()
    {
        UnitData newUnit = UnitData.CreateInstance<UnitData>();
        newUnit.Position = Position;
        newUnit.Type = Type;
        newUnit.Faction = Faction;
        newUnit.MaxHealth = MaxHealth;
        newUnit.CurrentHealth = CurrentHealth;

        return newUnit;
    }
}
