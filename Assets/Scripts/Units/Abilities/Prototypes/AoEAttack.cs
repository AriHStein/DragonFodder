using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoEAttack", menuName = "Units/Abilities/AoEAttack", order = 116)]
public class AoEAttack : AbilityPrototype
{
    public override string AnimationTrigger { get { return null; } }

    public enum TargetPriorityMode { MostHit, MostDamage, MostKills, HighestCurrentHealth, HighestDifficulty }

    public int PrimaryDamage = 1;
    public int AoEDamage = 1;
    public float Range = 1f;
    public float AoESize = 1f;
    public bool FriendlyFire = false;
    public GameObject ProjectilePrefab = null;
    public Status PrimaryStatus = default;
    public Status AoEStatus = default;
    public TargetPriorityMode TargetMode = TargetPriorityMode.MostHit;
    public bool TargetOtherFaction = true;

    public override AbilityInstance GetInstance()
    {
        return new AoEAttackInstance(this);
    }
}
