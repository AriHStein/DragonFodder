using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBase : Unit
{
    //[SerializeField] GameObject m_attackPrefab = default;
    //[SerializeField] float m_range = 4f;
    //[SerializeField] float m_spellStunTime = 5f;

    //Unit m_target;

    //public override void DoTurn()
    //{
        //if (m_target == null)
        //{
        //    //ChooseTarget();
        //    if (m_target == null)
        //    {
        //        return;
        //    }
        //}

        //FaceToward(m_target.Square);
        //if(CanCastSpell(m_target))
        //{
        //    CastSpell();
        //    return;
        //}
        //else
        //if (IsInAttackRange(m_target))
        //{
        //    Attack(m_target);
        //    return;
        //} 
        //else
        //{
        //    if (!TryMoveToward(m_target.Square))
        //    {
        //        if (TryGetPathTo(m_target.Square))
        //        {
        //            TryMoveToward(path.Dequeue());
        //        }
        //    }
        //}
    //}

    //protected override void Attack(Unit target)
    //{
    //    if(target == null)
    //    {
    //        return;
    //    }

    //    base.Attack(target);
    //    GameObject fireball = Instantiate(m_attackPrefab, transform.position + Vector3.up, Quaternion.identity);
    //    fireball.GetComponent<Projectile>().Initialize(target);
    //}

    //protected bool IsInAttackRange(Unit target)
    //{
    //    return target != null && 
    //        Vector2Int.Distance(target.Square.Position, Square.Position) < m_range;
    //}

    //protected override bool CanCastSpell(Unit target)
    //{
    //    return base.CanCastSpell(target) && 
    //        target != null && 
    //        IsInAttackRange(target);
    //}

    //protected override void CastSpell()
    //{
    //    //if(!CanCastSpell())
    //    //{
    //    //    return false;
    //    //}

    //    //Debug.Log($"Cast spell. Current MP: {CurrentMP}");
    //    base.CastSpell();
    //    m_target.Stun(m_spellStunTime);
    //}

    //void ChooseTarget()
    //{
    //    Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
    //    m_target = Board.Current.UnitManager.GetNearestUnitOfFaction(targetFaction, Square);

    //    if (m_target == null)
    //    {
    //        Debug.LogWarning("No Target found");
    //        return;
    //    }

    //    m_target.DeathEvent += OnTargetDeath;
    //}

    //void OnTargetDeath(Unit unit)
    //{
    //    if (m_target != unit)
    //    {
    //        Debug.LogWarning($"Dead unit is not m_target ({m_target})");
    //        return;
    //    }

    //    m_target = null;
    //}

    //public override void TakeDamage(int amount)
    //{
    //    base.TakeDamage(amount);
    //    Debug.Log($"Take {amount} damage. Remaining health: {m_currentHealth}");
    //}


    //protected override void Die()
    //{
    //    base.Die();
    //    Debug.Log("Die");
    //}
}
