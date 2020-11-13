using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;


[CreateAssetMenu(fileName = "Move", menuName = "Units/Abilities/Move", order = 116)]
public class Move : AbilityPrototype
{
    public override string AnimationTrigger { get { return null; } }
    
    public enum DestinationPriorityMode { Nearest, Farthest, Strongest, Weakest, MostHealth, LeastHealth, MostDamaged, LeastDamaged }
    public DestinationPriorityMode PriorityMode = DestinationPriorityMode.Nearest;
    public float MinStoppingDistance;
    public float MaxStoppingDistance;

    public override AbilityInstance GetInstance()
    {
        return new MoveInstance(this);
    }
}
