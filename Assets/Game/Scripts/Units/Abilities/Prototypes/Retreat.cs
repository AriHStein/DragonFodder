using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[CreateAssetMenu(fileName = "new Retreat", menuName = "Units/Abilities/Retreat", order = 116)]
public class Retreat : AbilityPrototype
{
    public override string AnimationTrigger { get { return null; } }
    public Threatened ThreatTest;


    public override AbilityInstance GetInstance()
    {
        return new RetreatInstance(this);
    }
}
