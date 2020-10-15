using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBase : Unit
{
    [SerializeField] float m_range = 2f;
    [SerializeField] int m_healAmount = 1;

    Unit m_target;
    public override void DoTurn()
    {
        if (m_target == null)
        {
            ChooseTarget();
            if (m_target == null)
            {
                return;
            }
        }

        if(CanCastSpell(m_target))
        {
            CastSpell();
            return;
        }
        else 
        if(Vector2Int.Distance(m_target.Square.Position, Square.Position) <= m_range)
        {
            FaceToward(m_target.Square);
            HealTarget();
        }
        else
        {
            FaceToward(m_target.Square);
            if (!TryMoveToward(m_target.Square))
            {
                if (TryGetPathTo(m_target.Square))
                {
                    TryMoveToward(path.Dequeue());
                }
            }
        }
    }

    void ChooseTarget()
    {
        List<Unit> possibleTargets = Board.Current.UnitManager.GetUnitsOfFaction(Faction);
        if(possibleTargets == null)
        {
            return;
        }

        Unit mostDamaged = null;
        int damage = 1;
        foreach(Unit unit in possibleTargets)
        {
            if(unit.MaxHealth - unit.CurrentHealth >= damage)
            {
                mostDamaged = unit;
                damage = unit.MaxHealth - unit.CurrentHealth;
            }
        }

        if(mostDamaged == null)
        {
            return;
        }

        m_target = mostDamaged;
        m_target.DeathEvent += OnTargetDeath;
    }

    void OnTargetDeath(Unit unit)
    {
        if (m_target != unit)
        {
            Debug.LogWarning("Dead unit is not m_target");
            return;
        }

        m_target = null;

    }

    void HealTarget()
    {
        HealUnit(m_target);
        //m_target.ChangeHealth(m_healAmount);
        if(m_target.CurrentHealth == m_target.MaxHealth)
        {
            m_target = null;
        }
    }

    void HealUnit(Unit unit)
    {
        unit.ChangeHealth(m_healAmount);
    }

    protected override void CastSpell()
    {
        base.CastSpell();
        List<Unit> allies = Board.Current.UnitManager.GetUnitsOfFaction(Faction);
        foreach(Unit unit in allies)
        {
            HealUnit(unit);
        }
    }
}
