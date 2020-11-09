using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[CreateAssetMenu(fileName = "new Retreat", menuName = "Units/Abilities/Retreat", order = 116)]
public class Retreat : Ability
{
    public override string AnimationTrigger { get { return null; } }
    [SerializeField] Threatened m_threatTest = default;

    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if (result != null)
        {
            return result;
        }

        Path_AStar<BoardSquare> path =
            board.GetPath(
                unit.Square,
                unit.Proto.Flying,
                (s) => { return m_threatTest.IsMetAt(s, board); }
            );

        return MoveAlongPath(path, unit, board);

    }

    IAbilityContext MoveAlongPath(Path_AStar<BoardSquare> path, Unit unit, Board board)
    {
        if (path == null || path.Length() == 0)
        {
            return new EmptyContext();
        }

        BoardSquare square = null;
        BoardSquare last = null;
        float dist = 0f;
        while (dist <= unit.Proto.MoveSpeed)
        {
            last = square;
            square = path.Dequeue();
            dist += 1f;

            if (path.Length() == 0)
            {
                return new SingleBoardSquareContext(AbilityPriority, unit, square, board);
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

        return new SingleBoardSquareContext(AbilityPriority, unit, square, board);
    }

    public override void Execute(IAbilityContext context)
    {
        if(!(context is SingleBoardSquareContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }
        
        ctx.Actor.FaceToward(ctx.Square);
        ctx.Board.TryMoveUnitTo(ctx.Actor, ctx.Square);
    }
}
