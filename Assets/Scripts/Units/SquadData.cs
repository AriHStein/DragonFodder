using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Squad", menuName = "Units/SquadData", order = 101)]
public class SquadData : ScriptableObject
{

    public List<UnitData> Units;
    public Vector2Int SquadOrigin;

    public Vector2Int Size;

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

        Size = max;
    }
}
