using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class MoveInstance : AbilityInstance
{
    Move.DestinationPriorityMode m_priorityMode;
    float m_minStoppingDistance;
    float m_maxStoppingDistance;

    public MoveInstance(Move proto) : base(proto)
    {
        m_priorityMode = proto.PriorityMode;
        m_minStoppingDistance = proto.MinStoppingDistance;
        m_maxStoppingDistance = proto.MaxStoppingDistance;
    }

    protected override IAbilityContext GetValueOverride(Unit unit, Board_Base board)
    {
        BoardSquare dest = null;
        switch (m_priorityMode)
        {
            case Move.DestinationPriorityMode.Nearest:
                dest = board.UnitManager.GetNearestUnitOfFaction(unit.Faction.Opposite(), unit.Square).Square;
                break;

            case Move.DestinationPriorityMode.Farthest:
                dest = board.UnitManager.GetFarthestUnitOfFaction(unit.Faction.Opposite(), unit.Square).Square;
                break;

            case Move.DestinationPriorityMode.Strongest:
                List<Unit> strongest = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                strongest.OrderByDescending(u => u.Difficulty);
                dest = strongest[0].Square;
                break;

            case Move.DestinationPriorityMode.Weakest:
                List<Unit> weakest = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                weakest.OrderBy(u => u.Difficulty);
                dest = weakest[0].Square;
                break;

            case Move.DestinationPriorityMode.MostHealth:
                List<Unit> mostHealth = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                mostHealth.OrderByDescending(u => u.CurrentHealth);
                dest = mostHealth[0].Square;
                break;

            case Move.DestinationPriorityMode.LeastHealth:
                List<Unit> leastHealth = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                leastHealth.OrderBy(u => u.CurrentHealth);
                dest = leastHealth[0].Square;
                break;

            case Move.DestinationPriorityMode.MostDamaged:
                List<Unit> mostDamaged = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                mostDamaged.OrderByDescending(u => u.MaxHealth - u.CurrentHealth);
                dest = mostDamaged[0].Square;
                break;

            case Move.DestinationPriorityMode.LeastDamaged:
                List<Unit> leastDamaged = board.UnitManager.GetUnitsOfFaction(unit.Faction.Opposite());
                leastDamaged.OrderBy(u => u.MaxHealth - u.CurrentHealth);
                dest = leastDamaged[0].Square;
                break;
        }

        Path_AStar<BoardSquare> path =
            board.GetPath(
                unit.Square,
                unit.Flying,
                (s) => {
                    float distance = Vector2Int.Distance(s.Position, dest.Position);
                    return distance >= m_minStoppingDistance && distance <= m_maxStoppingDistance;
                },
                (s) => { return Vector2Int.Distance(s.Position, dest.Position); }
            );

        return MoveAlongPath(path, unit, board);
    }

    IAbilityContext MoveAlongPath(Path_AStar<BoardSquare> path, Unit unit, Board_Base board)
    {
        if (path == null || path.Length() == 0)
        {
            Debug.Log("No path");
            return new EmptyContext();
        }

        BoardSquare square = null;
        BoardSquare last = null;
        float dist = 0f;
        while (dist <= unit.MoveSpeed)
        {
            last = square;
            square = path.Dequeue();
            dist += 1f;

            if (path.Length() == 0)
            {
                return new SingleBoardSquareContext(m_abilityPriority, unit, square, board);
            }
        }

        // for flying units, an intermediate square on the path may be occupied. 
        // If so, go back one square on the path. If that fails, movement fails.
        if (square.Unit != null)
        {
            if (last.Unit != null)
            {
                return new EmptyContext();
            }

            square = last;
        }

        return new SingleBoardSquareContext(m_abilityPriority, unit, square, board);
    }

    public override void Execute(IAbilityContext context)
    {
        if (!(context is SingleBoardSquareContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        ctx.Actor.FaceToward(ctx.Square);
        ctx.Board.TryMoveUnitTo(ctx.Actor, ctx.Square);
    }


}
