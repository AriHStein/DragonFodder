using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttackInstance : AbilityInstance
{
    int m_primaryDamage = 1;
    int m_aoeDamage = 1;
    float m_range = 1f;
    float m_aoeRange = 1f;
    bool m_friendlyFire = false;
    GameObject m_projectilePrefab = null;
    Status m_primaryStatus = default;
    Status m_aoeStatus = default;
    AoEAttack.TargetPriorityMode m_targetPriorityMode;
    bool m_targetOtherFaction = true;

    public AoEAttackInstance(AoEAttack proto) : base(proto)
    {
        m_primaryDamage = proto.PrimaryDamage;
        m_aoeDamage = proto.AoEDamage;
        m_range = proto.Range;
        m_aoeRange = proto.AoESize;
        m_friendlyFire = proto.FriendlyFire;
        m_projectilePrefab = proto.ProjectilePrefab;
        m_primaryStatus = proto.PrimaryStatus;
        m_aoeStatus = proto.AoEStatus;
        m_targetPriorityMode = proto.TargetMode;
        m_targetOtherFaction = proto.TargetOtherFaction;
    }

    protected override IAbilityContext GetValueOverride(Unit unit, Board board)
    {
        List<BoardSquare> squaresInRange = GetPossibleTargets(unit, board);
        if (squaresInRange.Count == 0)
        {
            return new EmptyContext();
        }

        TargetData target = null;
        Faction targetFaction = m_targetOtherFaction ? unit.Faction.Opposite() : unit.Faction;

        foreach (BoardSquare square in squaresInRange)
        {
            target = CompareTargets(target, new TargetData(square, board, m_targetPriorityMode, m_aoeRange, m_primaryDamage, m_aoeDamage, targetFaction, m_friendlyFire));
        }

        if (target == null)
        {
            return new EmptyContext();
        }

        return new SingleBoardSquareContext(m_abilityPriority, unit, target.Target, board);
    }

    List<BoardSquare> GetPossibleTargets(Unit unit, Board board)
    {
        List<BoardSquare> squares = board.GetSquaresInRange(unit.Square.Position, m_aoeRange);
        for (int i = squares.Count; i > 0; i--)
        {
            if (!CanTargetSquare(unit, squares[i - 1]))
            {
                squares.RemoveAt(i - 1);
            }
        }

        return squares;
    }

    private class TargetData
    {
        public BoardSquare Target;
        public int Comparer;

        public TargetData(BoardSquare target, Board board, AoEAttack.TargetPriorityMode mode, float aoeRange, int primaryDamage, int aoeDamage, Faction targetFaction, bool friendlyFire = false)
        {
            Target = target;

            List<BoardSquare> squares = board.GetSquaresInRange(target.Position, aoeRange);
            foreach (BoardSquare square in squares)
            {
                if (square.Unit == null ||
                    !square.Unit.IsTargetable() ||
                    (square.Unit.Faction != targetFaction && !friendlyFire))
                {
                    continue;
                }

                // count friendly fire hits/damage/etc as negative
                int sign = square.Unit.Faction == targetFaction ? 1 : -1;
                switch (mode)
                {
                    case AoEAttack.TargetPriorityMode.MostHit:
                        Comparer += sign;
                        break;

                    case AoEAttack.TargetPriorityMode.MostDamage:
                        Comparer += target == square ? sign * primaryDamage : sign * aoeDamage;
                        break;

                    case AoEAttack.TargetPriorityMode.MostKills:
                        int damage = target == square ? primaryDamage : aoeDamage;
                        if (damage > square.Unit.CurrentHealth)
                        {
                            Comparer += sign;
                        }
                        break;

                    case AoEAttack.TargetPriorityMode.HighestCurrentHealth:
                        Comparer += sign * square.Unit.CurrentHealth;
                        break;

                    case AoEAttack.TargetPriorityMode.HighestDifficulty:
                        Comparer += sign * square.Unit.Difficulty;
                        break;

                    default:
                        Debug.Log($"TargetPriorityMode {mode} not implemented.");
                        break;
                }
            }
        }
    }

    TargetData CompareTargets(TargetData currentTarget, TargetData otherTarget)
    {
        if (otherTarget == null)
        {
            Debug.LogError("Unable to compare to a null target.");
            return currentTarget;
        }

        if (otherTarget.Comparer <= 0)
        {
            // This may return null. That's okay.
            return currentTarget;
        }

        if (currentTarget == null)
        {
            // other target is valid and current is null, so return other.
            return otherTarget;
        }

        // return better of two targets
        return otherTarget.Comparer > currentTarget.Comparer ? otherTarget : currentTarget;
    }

    public override void Execute(IAbilityContext context)
    {
        if (!(context is SingleBoardSquareContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        if (m_projectilePrefab != null)
        {
            GameObject projectile = GameObject.Instantiate(m_projectilePrefab, ctx.Actor.transform);
            projectile.GetComponent<Projectile>().Initialize(ctx.Square.transform);
        }

        ctx.Actor.FaceToward(ctx.Square);
        List<BoardSquare> hitSquares = ctx.Board.GetSquaresInRange(ctx.Square.Position, m_aoeRange);
        foreach (BoardSquare square in hitSquares)
        {
            if (square.Unit == null ||
                !square.Unit.IsTargetable() ||
                (((square.Unit.Faction != ctx.Actor.Faction.Opposite()) == m_targetOtherFaction) && !m_friendlyFire))
            {
                continue;
            }

            if (square == ctx.Square)
            {
                if (m_primaryStatus != null)
                {
                    square.Unit.ApplyStatus(m_primaryStatus.GetInstance());
                }

                square.Unit.ChangeHealth(-m_primaryDamage);
            }
            else
            {
                if (m_aoeStatus != null)
                {
                    square.Unit.ApplyStatus(m_aoeStatus.GetInstance());
                }

                square.Unit.ChangeHealth(-m_aoeDamage);
            }
        }

        base.Execute(context);
    }

    public override bool CanTargetUnit(Unit unit, Unit other)
    {
        return other.IsTargetable() &&
            ((other.Faction == unit.Faction.Opposite()) == m_targetOtherFaction) &&
            CanTargetSquare(unit, other.Square);
    }

    public override bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return target != null && Vector2Int.Distance(unit.Square.Position, target.Position) <= m_range;
    }
}
