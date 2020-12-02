using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new Unit Prototype", menuName = "Units/UnitPrototype", order = 111)]
public class UnitPrototype : ScriptableObject
{
    public string Type;
    public int RecruitCost;
    public int MaxHealth;
    public int MaxMP;
    //public int SpellMP;
    //public int AttackDamage;
    //public float AttackRange = 1f;
    public float TimeBetweenActions;

    public int Difficulty = 1;
    public bool Flying = false;
    public float MoveSpeed = 1f;

    public List<AbilityPrototype> BaseAbilities = default;

    public GameObject Prefab;
}

