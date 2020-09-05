using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UnitData", menuName = "Units/UnitData", order = 100)]
public class UnitData : ScriptableObject
{
    public Vector2Int Position;
    public GameObject Prefab;
    public Faction Faction;
    public int MaxHealth;
    public int CurrentHealth;

    public UnitData(Unit unit)
    {
        Position = unit.Square.Position;
        Prefab = unit.gameObject;
        Faction = unit.Faction;

        MaxHealth = unit.MaxHealth;
        CurrentHealth = unit.CurrentHealth;
    }
}
