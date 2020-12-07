using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon", menuName = "Units/Abilities/Summon", order = 116)]
public class Summon : AbilityPrototype
{
    public override string AnimationTrigger { get { return null; } }

    public UnitPrototype SummonPrototype;
    
    public override AbilityInstance GetInstance()
    {
        return new SummonInstance(this);
    }

}
