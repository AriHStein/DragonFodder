using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoEAttack", menuName = "Units/Abilities/AoEAttack", order = 116)]
public class AoEAttack : Ability
{
    public override string AnimationTrigger { get { return null; } }

    public enum TargetPriorityMode { MostHit, MostDamage, MostKills, HighestCurrentHealth, HighestDifficulty }

    [SerializeField] int m_primaryDamage = 1;
    [SerializeField] int m_aoeDamage = 1;
    [SerializeField] float m_range = 1f;
    [SerializeField] float m_aoeRange = 1f;
    [SerializeField] bool m_friendlyFire = false;
    [SerializeField] GameObject m_projectilePrefab = null;
    [SerializeField] Status m_primaryStatus = default;
    [SerializeField] Status m_aoeStatus = default;
    [SerializeField] TargetPriorityMode m_targetPriorityMode = TargetPriorityMode.MostHit;
    [SerializeField] Faction m_targetFaction = Faction.Enemy;

    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if (result != null)
        {
            return result;
        }

        List<BoardSquare> squaresInRange = GetPossibleTargets(unit, board);
        if(squaresInRange.Count == 0)
        {
            return new EmptyContext();
        }

        TargetData target = null;
        foreach(BoardSquare square in squaresInRange)
        {
            target = CompareTargets(target, new TargetData(square, board, m_targetPriorityMode, m_aoeRange, m_primaryDamage, m_aoeDamage, m_targetFaction, m_friendlyFire));
        }
        
        if(target == null)
        {
            return new EmptyContext();
        }

        return new SingleBoardSquareContext(AbilityPriority, unit, target.Target, board);
    }

    List<BoardSquare> GetPossibleTargets(Unit unit, Board board)
    {
        //List<BoardSquare> squares = new List<BoardSquare>();
        //for(int x = -Mathf.CeilToInt(m_range); x <= Mathf.CeilToInt(m_range); x++)
        //{
        //    for(int y = -Mathf.CeilToInt(m_range); y <= Mathf.CeilToInt(m_range); y++)
        //    {
        //        if (x * x + y * y > m_range * m_range)
        //        {
        //            continue;
        //        }

        //        Vector2Int off = new Vector2Int(x, y);
        //        BoardSquare square = board.GetSquareAt(unit.Square.Position + off);
        //        if(CanTargetSquare(unit, square))
        //        {
        //            squares.Add(square);
        //        }
        //    }
        //}

        List<BoardSquare> squares = board.GetSquaresInRange(unit.Square.Position, m_aoeRange);
        for(int i = squares.Count; i > 0; i--)
        {
            if(!CanTargetSquare(unit, squares[i-1]))
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
        //public int Difficulty;
        //public int TotalCurrentHealth;
        //public int Damage;
        //public int Kills;

        public TargetData(BoardSquare target, Board board, TargetPriorityMode mode, float aoeRange, int primaryDamage, int aoeDamage, Faction targetFaction, bool friendlyFire = false)
        {
            Target = target;
            
            List<BoardSquare> squares = board.GetSquaresInRange(target.Position, aoeRange);
            foreach(BoardSquare square in squares)
            {
                if(square.Unit == null ||
                    !square.Unit.IsTargetable() ||
                    (square.Unit.Faction != targetFaction && !friendlyFire))
                {
                    continue;
                }

                // count friendly fire hits/damage/etc as negative
                int sign = square.Unit.Faction == targetFaction ? 1 : -1;
                switch (mode)
                {
                    case TargetPriorityMode.MostHit:
                        Comparer += sign;
                        break;

                    case TargetPriorityMode.MostDamage:
                        Comparer += target == square ? sign * primaryDamage : sign * aoeDamage;
                        break;

                    case TargetPriorityMode.MostKills:
                        int damage = target == square ? primaryDamage : aoeDamage;
                        if(damage > square.Unit.CurrentHealth)
                        {
                            Comparer += sign;
                        }
                        break;

                    case TargetPriorityMode.HighestCurrentHealth:
                        Comparer += sign * square.Unit.CurrentHealth;
                        break;

                    case TargetPriorityMode.HighestDifficulty:
                        Comparer += sign * square.Unit.Proto.Difficulty;
                        break;

                    default:
                        Debug.Log($"TargetPriorityMode {mode} not implemented.");
                        break;
                }

                //Difficulty += square.Unit.Proto.Difficulty;
                //TotalCurrentHealth += square.Unit.CurrentHealth;
                //int damage = target == square ? primaryDamage : aoeDamage;
                //if(damage >= square.Unit.CurrentHealth)
                //{
                //    Damage += square.Unit.CurrentHealth;
                //    Kills++;
                //} 
                //else
                //{
                //    Damage += damage;
                //}
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

        if(currentTarget == null)
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

        //SingleTargetContext ctx = (SingleTargetContext)context;
        if (m_projectilePrefab != null)
        {
            GameObject projectile = Instantiate(m_projectilePrefab, ctx.Actor.transform);
            projectile.GetComponent<Projectile>().Initialize(ctx.Square.transform);
        }

        List<BoardSquare> hitSquares = ctx.Board.GetSquaresInRange(ctx.Square.Position, m_aoeRange);
        foreach(BoardSquare square in hitSquares)
        {
            if (square.Unit == null ||
                !square.Unit.IsTargetable() ||
                (square.Unit.Faction != m_targetFaction && !m_friendlyFire))
            {
                continue;
            }

            if(square == ctx.Square)
            {
                if(m_primaryStatus != null)
                {
                    square.Unit.ApplyStatus(m_primaryStatus.GetInstance());
                }

                square.Unit.ChangeHealth(-m_primaryDamage);
            }
            else
            {
                if(m_aoeStatus != null)
                {
                    square.Unit.ApplyStatus(m_aoeStatus.GetInstance());
                }

                square.Unit.ChangeHealth(-m_aoeDamage);
            }
        }
        //if (m_primaryStatus != null)
        //{
        //    ctx.Target.ApplyStatus(m_status.GetInstance());
        //}

        base.Execute(context);
    }
    
    public override bool CanTargetUnit(Unit unit, Unit other)
    {
        return other.IsTargetable() && 
            other.Faction == m_targetFaction && 
            CanTargetSquare(unit, other.Square);
    }
    
    public override bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return target != null && Vector2Int.Distance(unit.Square.Position, target.Position) <= m_range;
    }
}
