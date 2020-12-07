using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetContext : IAbilityContext
{
    public Unit Actor { get; protected set; }
    public int Value { get; protected set; }
    public Unit Target { get; protected set; }

    public SingleTargetContext(Unit actor, int value, Unit target)
    {
        Actor = actor;
        Value = value;
        Target = target;
    }
}
