﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


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

    public override List<GameObject> VFX { 
        get 
            { 
            if(ProjectilePrefab == null)
            {
                return null;
            }
            return new List<GameObject> { ProjectilePrefab }; 
        } 
    }

    public override AbilityInstance GetInstance()
    {
        return new SingleTargetAttackInstance(this);
    }
}
