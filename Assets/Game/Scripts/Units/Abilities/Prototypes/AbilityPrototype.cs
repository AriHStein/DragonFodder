using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbilityPrototype : ScriptableObject
{
    //[SerializeField] string m_name = default;
    //public string Name;

    //[SerializeField] List<AbilityPrototype> m_unlockedAbilities = default;
    //public List<AbilityPrototype> UnlockedAbilities; //{ get { return m_unlockedAbilities; } }

    [SerializeField] string m_abilityName = default;
    public string Name { get { return m_abilityName; } }
    public abstract string AnimationTrigger { get; }
    
    //public int m_abilityPriority = 1;
    public List<Condition> Conditions; //= default;
    public int MPCost = 0;
    public int AbilityPriority; //{ get { return m_abilityPriority; } }

    public abstract AbilityInstance GetInstance();

    public virtual List<GameObject> VFX { get { return null; } }

    //public abstract void Reset();
    //public virtual IAbilityContext GetValue(Unit unit, Board board)
    //{
    //    if(unit.CurrentMP < MPCost)
    //    {
    //        return new EmptyContext();
    //    }
        
    //    foreach(Condition condition in Conditions)
    //    {
    //        if(!condition.IsMet(unit, board))
    //        {
    //            return new EmptyContext();
    //        }
    //    }

    //    return null;
    //}

    //public event Action<AbilityPrototype> AbilityExecutedEvent;
    //public virtual void Execute(IAbilityContext context)
    //{
    //    context.Actor.ChangeMP(-MPCost);
    //    AbilityExecutedEvent?.Invoke(this);
    //}

    //public virtual bool CanTargetUnit(Unit unit, Unit other)
    //{
    //    return false;
    //}

    //public virtual bool CanTargetSquare(Unit unit, BoardSquare target)
    //{
    //    return false;
    //}
}