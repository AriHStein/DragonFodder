using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbilityInstance
{
    public string AnimationTrigger { get; protected set; }

    public bool Unlocked { get; protected set; }

    protected int m_abilityPriority;
    protected int m_mpCost;
    protected List<Condition> m_conditions;
    
    public AbilityInstance(AbilityPrototype proto)
    {
        AnimationTrigger = proto.AnimationTrigger;
        m_abilityPriority = proto.AbilityPriority;

        m_mpCost = proto.MPCost;
        m_conditions = proto.Conditions;
    }

    public IAbilityContext GetValue(Unit unit, Board board)
    {
        if (unit.CurrentMP < m_mpCost)
        {
            return new EmptyContext();
        }

        foreach (Condition condition in m_conditions)
        {
            if (!condition.IsMet(unit, board))
            { 
                return new EmptyContext();
            }
        }

        return GetValueOverride(unit, board);
    }

    protected abstract IAbilityContext GetValueOverride(Unit unit, Board board);

    public event Action<AbilityInstance> AbilityExecutedEvent;
    public virtual void Execute(IAbilityContext context)
    {
        context.Actor.ChangeMP(-m_mpCost);
        AbilityExecutedEvent?.Invoke(this);
    }

    public virtual bool CanTargetUnit(Unit unit, Unit other)
    {
        return false;
    }

    public virtual bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return false;
    }
}
