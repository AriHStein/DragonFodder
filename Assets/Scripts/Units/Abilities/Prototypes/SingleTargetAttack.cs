using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "SingleTargetAttack", menuName = "Units/Abilities/SingleTargetAttack", order = 116)]
public class SingleTargetAttack : AbilityPrototype
{
    [SerializeField] string m_animationTriggerOverride = null;
    public override string AnimationTrigger { 
        get { 
            if(m_animationTriggerOverride != null && m_animationTriggerOverride != "")
            {
                return m_animationTriggerOverride;
            }

            return "Attack"; 
        } 
    }
    public enum TargetPriorityMode { Strongest, Weakest, HighestHealth, FewestHitsToKill, Nearest, Farthest }

    public int Damage = 1;
    public float Range = 1f;
    public GameObject ProjectilePrefab = null;
    public Status Status = default;
    public TargetPriorityMode TargetMode = TargetPriorityMode.FewestHitsToKill;
    public bool TargetOtherFaction = true;

    public override AbilityInstance GetInstance()
    {
        return new SingleTargetAttackInstance(this);
    }
}
