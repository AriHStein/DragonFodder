using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RetreatInstance : AbilityInstance
{

    Threatened m_threatTest = default;

    public RetreatInstance(Retreat proto) : base(proto)
    {
        m_threatTest = proto.ThreatTest;
    }

    protected override IAbilityContext GetValueOverride(Unit unit, Board_Base board)
    {
        Path_AStar<BoardSquare> path =
            board.GetPath(
                unit.Square,
                unit.Flying,
                (s) => { return m_threatTest.IsMetAt(s, board); }
            );

        return MoveAlongPath(path, unit, board);

    }

    IAbilityContext MoveAlongPath(Path_AStar<BoardSquare> path, Unit unit, Board_Base board)
    {
        if (path == null || path.Length() == 0)
        {
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
