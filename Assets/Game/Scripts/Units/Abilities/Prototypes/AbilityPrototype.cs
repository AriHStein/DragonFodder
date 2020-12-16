using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbilityPrototype : ScriptableObject
{
    [SerializeField] string m_abilityName = default;
    public string Name { get { return m_abilityName; } }
    public abstract string AnimationTrigger { get; }
    
    public List<Condition> Conditions;
    public int MPCost = 0;
    public int AbilityPriority;

    public abstract AbilityInstance GetInstance();

    public virtual List<GameObject> VFX { get { return null; } }
}