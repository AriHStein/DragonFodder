using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UnitData", menuName = "Units/UnitData", order = 100)]
public class UnitData : ScriptableObject
{
    public Vector2Int Position;
    public GameObject Prefab;
    public Faction Faction;
}
