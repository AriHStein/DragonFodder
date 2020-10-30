using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Attack", menuName = "Units/Abilities/Attack", order = 116)]
public class Attack : Ability
{
    public override string AnimationTrigger { get { return "Attack"; } }
    public enum TargetPriorityMode { Strongest, Weakest, HighestHealth, FewestHitsToKill, Nearest, Farthest }

    [SerializeField] int m_damage = 1;
    [SerializeField] float m_range = 1f;
    [SerializeField] GameObject m_projectilePrefab = null;
    [SerializeField] Status m_status = default;
    [SerializeField] TargetPriorityMode m_targetPriorityMode = TargetPriorityMode.FewestHitsToKill;
    [SerializeField] Faction m_targetFaction = Faction.Enemy;

    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if (result != null)
        {
            return result;
        }

        List<Unit> enemiesInRange = GetPossibleTargets(unit, board);
        if(enemiesInRange.Count == 0)
        {
            return new EmptyContext();
        }

        Unit target = null;
        foreach(Unit enemy in enemiesInRange)
        {
            target = CompareTargets(unit, target, enemy);
        }

        return new SingleTargetContext(unit, AbilityPriority, target);
    }

    Unit CompareTargets(Unit attacker, Unit currentTarget, Unit otherTarget)
    {
        if(currentTarget == null)
        {
            if(otherTarget != null)
            {
                return otherTarget;
            }
            
            Debug.LogError("Unable to compare null targets.");
            return null;
        }

        if (otherTarget == null)
        {
            Debug.LogError("Unable to compare to a null target.");
            return currentTarget;
        }

        switch (m_targetPriorityMode) 
        {
            case TargetPriorityMode.Strongest:
                return otherTarget.Proto.Difficulty > currentTarget.Proto.Difficulty ? otherTarget : currentTarget;

            case TargetPriorityMode.Weakest:
                return otherTarget.Proto.Difficulty < currentTarget.Proto.Difficulty ? otherTarget : currentTarget;

            case TargetPriorityMode.HighestHealth:
                return otherTarget.CurrentHealth > currentTarget.CurrentHealth ? otherTarget : currentTarget;

            case TargetPriorityMode.FewestHitsToKill:
                if(otherTarget.CurrentHealth / m_damage < currentTarget.CurrentHealth / m_damage)
                {
                    return otherTarget;
                }

                if(otherTarget.CurrentHealth / m_damage == currentTarget.CurrentHealth / m_damage)
                {
                    return otherTarget.CurrentHealth > currentTarget.CurrentHealth ? otherTarget : currentTarget;
                }

                return currentTarget;

            case TargetPriorityMode.Nearest:
                return Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) < Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) ?
                    otherTarget : currentTarget;

            case TargetPriorityMode.Farthest:
                return Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) > Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) ?
                    otherTarget : currentTarget;

            default:
                return currentTarget;
        }

    }

    public override void Execute(IAbilityContext context)
    {
        if(!(context is SingleTargetContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }
        
        //SingleTargetContext ctx = (SingleTargetContext)context;
        ctx.Target.ChangeHealth(-m_damage);
        if(m_projectilePrefab != null)
        {
            GameObject projectile = Instantiate(m_projectilePrefab, ctx.Actor.transform);
            projectile.GetComponent<Projectile>().Initialize(ctx.Target);
        }
        if(m_status != null)
        {
            ctx.Target.ApplyStatus(m_status.GetInstance());
        }
        base.Execute(context);
    }

    public override bool CanTargetUnit(Unit unit, Unit other)
    {
        return other.IsTargetable() && 
            other.Faction != unit.Faction &&
           CanTargetSquare(unit, other.Square);
    }

    public override bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return Vector2Int.Distance(unit.Square.Position, target.Position) <= m_range;
    }


    List<Unit> GetPossibleTargets(Unit unit, Board board)
    {
        Vector2Int unitPos = unit.Square.Position;
        List<Unit> possibleTargets = new List<Unit>();

        //Vector2Int[] directions = new Vector2Int[]
        //{
        //    Vector2Int.up,
        //    Vector2Int.right,
        //    Vector2Int.down,
        //    Vector2Int.left
        //};

        for(int x = Mathf.FloorToInt(-m_range); x <= Mathf.CeilToInt(m_range); x++)
        {
            for(int y = Mathf.FloorToInt(-m_range); y <= Mathf.CeilToInt(m_range); y++)
            {
                if(x*x + y*y > m_range * m_range)
                {
                    continue;
                }
                
                BoardSquare square = board.GetSquareAt(unitPos + new Vector2Int(x, y));
                if (square != null && square.Unit != null && square.Unit.Faction == m_targetFaction && square.Unit.IsTargetable())
                {
                    possibleTargets.Add(square.Unit);
                }
            }
        }

        //foreach(Vector2Int direction in directions)
        //{
        //    BoardSquare square = board.GetSquareAt(unitPos + direction);
        //    if(square != null && square.Unit != null && square.Unit.Faction != unit.Faction && square.Unit.IsTargetable())
        //    {
        //        possibleTargets.Add(square.Unit);
        //    }
        //}

        return possibleTargets;
    }
}
