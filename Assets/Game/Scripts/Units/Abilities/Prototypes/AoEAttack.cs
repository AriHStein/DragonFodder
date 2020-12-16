using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoEAttack", menuName = "Units/Abilities/AoEAttack", order = 116)]
public class AoEAttack : AbilityPrototype
{

    [SerializeField] string m_animationTriggerOverride = null;
    public override string AnimationTrigger
    {
        get {
            if (m_animationTriggerOverride != null && m_animationTriggerOverride != "")
            {
                return m_animationTriggerOverride;
            }

            return "Attack";
        }
    }

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

    public override List<GameObject> VFX
    {
        get {
            if (ProjectilePrefab == null)
            {
                return null;
            }
            return new List<GameObject> { ProjectilePrefab };
        }
    }
    public override AbilityInstance GetInstance()
    {
        return new AoEAttackInstance(this);
    }
}
