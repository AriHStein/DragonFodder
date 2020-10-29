using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Ability : ScriptableObject
{
    public abstract string AnimationTrigger { get; }
    
    [SerializeField] int m_abilityPriority = 1;
    [SerializeField] List<Condition> m_conditions = default;
    protected int AbilityPriority { get { return m_abilityPriority; } }

    //public abstract void Reset();
    public virtual IAbilityContext GetValue(Unit unit, Board board)
    {
        foreach(Condition condition in m_conditions)
        {
            if(!condition.IsMet(unit, board))
            {
                return new EmptyContext();
            }
        }

        return null;
    }

    public event Action<Ability> AbilityExecutedEvent;
    public virtual void Execute(IAbilityContext context)
    {
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
