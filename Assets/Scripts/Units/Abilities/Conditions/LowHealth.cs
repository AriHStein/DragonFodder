using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new LowHealthCondition", menuName ="Units/Conditions/LowHealth", order = 121)]
public class LowHealth : Condition
{
    [Range(0, 1)]
    public float Threshold;

    public override bool IsMet(Unit unit, Board board)
    {
        return unit.CurrentHealth < unit.MaxHealth * Threshold;
    }
}
