using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetAttackInstance : AbilityInstance
{
    int m_damage;
    float m_range;
    GameObject m_projectilePrefab;
    Status m_status;
    SingleTargetAttack.TargetPriorityMode m_targetPriorityMode;
    bool m_targetOtherFaction;

    public SingleTargetAttackInstance(SingleTargetAttack proto) : base(proto)
    {
        m_damage = proto.Damage;
        m_range = proto.Range;
        m_projectilePrefab = proto.ProjectilePrefab;
        m_status = proto.Status;
        m_targetPriorityMode = proto.TargetMode;
        m_targetOtherFaction = proto.TargetOtherFaction;
    }

    protected override IAbilityContext GetValueOverride(Unit unit, Board board)
    {
        List<Unit> enemiesInRange = GetPossibleTargets(unit, board);
        if (enemiesInRange.Count == 0)
        {
            return new EmptyContext();
        }

        Unit target = null;
        foreach (Unit enemy in enemiesInRange)
        {
            target = CompareTargets(unit, target, enemy);
        }

        return new SingleTargetContext(unit, m_abilityPriority, target);
    }

    Unit CompareTargets(Unit attacker, Unit currentTarget, Unit otherTarget)
    {
        if (currentTarget == null)
        {
            if (otherTarget != null)
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
            case SingleTargetAttack.TargetPriorityMode.Strongest:
                return otherTarget.Difficulty > currentTarget.Difficulty ? otherTarget : currentTarget;

            case SingleTargetAttack.TargetPriorityMode.Weakest:
                return otherTarget.Difficulty < currentTarget.Difficulty ? otherTarget : currentTarget;

            case SingleTargetAttack.TargetPriorityMode.HighestHealth:
                return otherTarget.CurrentHealth > currentTarget.CurrentHealth ? otherTarget : currentTarget;

            case SingleTargetAttack.TargetPriorityMode.FewestHitsToKill:
                if (otherTarget.CurrentHealth / m_damage < currentTarget.CurrentHealth / m_damage)
                {
                    return otherTarget;
                }

                if (otherTarget.CurrentHealth / m_damage == currentTarget.CurrentHealth / m_damage)
                {
                    return otherTarget.CurrentHealth > currentTarget.CurrentHealth ? otherTarget : currentTarget;
                }

                return currentTarget;

            case SingleTargetAttack.TargetPriorityMode.Nearest:
                return Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) < Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) ?
                    otherTarget : currentTarget;

            case SingleTargetAttack.TargetPriorityMode.Farthest:
                return Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) > Vector2Int.Distance(attacker.Square.Position, otherTarget.Square.Position) ?
                    otherTarget : currentTarget;

            default:
                return currentTarget;
        }
    }

    public override void Execute(IAbilityContext context)
    {
        if (!(context is SingleTargetContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        ctx.Actor.FaceToward(ctx.Target.Square);
        ctx.Target.ChangeHealth(-m_damage);
        if (m_projectilePrefab != null)
        {
            GameObject projectile = GameObject.Instantiate(m_projectilePrefab, ctx.Actor.transform);
            projectile.GetComponent<Projectile>().Initialize(ctx.Target.transform);
        }
        if (m_status != null)
        {
            ctx.Target.ApplyStatus(m_status.GetInstance());
        }
        base.Execute(context);
    }

    public override bool CanTargetUnit(Unit unit, Unit other)
    {
        return other.IsTargetable() &&
            (m_targetOtherFaction == (other.Faction == unit.Faction.Opposite())) &&
           CanTargetSquare(unit, other.Square);
    }

    public override bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return target != null && Vector2Int.Distance(unit.Square.Position, target.Position) <= m_range;
    }


    List<Unit> GetPossibleTargets(Unit unit, Board board)
    {
        Vector2Int unitPos = unit.Square.Position;
        List<Unit> possibleTargets = new List<Unit>();

        foreach (BoardSquare square in board.GetSquaresInRange(unitPos, m_range))
        {
            if (square.Unit != null &&
                (m_targetOtherFaction == (square.Unit.Faction == unit.Faction.Opposite())) &&
                square.Unit.IsTargetable())
            {
                possibleTargets.Add(square.Unit);
            }
        }
        //for(int x = Mathf.FloorToInt(-m_range); x <= Mathf.CeilToInt(m_range); x++)
        //{
        //    for(int y = Mathf.FloorToInt(-m_range); y <= Mathf.CeilToInt(m_range); y++)
        //    {
        //        if(x*x + y*y > m_range * m_range)
        //        {
        //            continue;
        //        }

        //        BoardSquare square = board.GetSquareAt(unitPos + new Vector2Int(x, y));
        //        if (square != null && square.Unit != null && square.Unit.Faction == m_targetFaction && square.Unit.IsTargetable())
        //        {
        //            possibleTargets.Add(square.Unit);
        //        }
        //    }
        //}

        return possibleTargets;
    }
}
