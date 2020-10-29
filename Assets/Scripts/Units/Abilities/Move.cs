using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;


[CreateAssetMenu(fileName = "new Move", menuName = "Units/Abilities/Move", order = 115)]
public class Move : Ability
{
    public override string AnimationTrigger { get { return null; } }
    
    public enum DestinationPriorityMode { Nearest, Farthest, Strongest, Weakest, MostHealth, LeastHealth }
    [SerializeField] DestinationPriorityMode m_priorityMode = DestinationPriorityMode.Nearest;
    [SerializeField] Faction m_targetFaction = Faction.Enemy;
    [SerializeField] float m_minDistance = 1f;
    [SerializeField] float m_maxDistance = 1f;

    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if(result != null)
        {
            return result;
        }
        
        BoardSquare dest = null;
        switch(m_priorityMode)
        {
            case DestinationPriorityMode.Nearest:
                dest = board.UnitManager.GetNearestUnitOfFaction(m_targetFaction, unit.Square).Square;
                break;

            case DestinationPriorityMode.Farthest:
                dest = board.UnitManager.GetFarthestUnitOfFaction(m_targetFaction, unit.Square).Square;
                break;

            case DestinationPriorityMode.Strongest:
                List<Unit> strongest = board.UnitManager.GetUnitsOfFaction(m_targetFaction);
                strongest.OrderByDescending(u => u.Proto.Difficulty);
                dest = strongest[0].Square;
                break;

            case DestinationPriorityMode.Weakest:
                List<Unit> weakest = board.UnitManager.GetUnitsOfFaction(m_targetFaction);
                weakest.OrderBy(u => u.Proto.Difficulty);
                dest = weakest[0].Square;
                break;

            case DestinationPriorityMode.MostHealth:
                List<Unit> mostHealth = board.UnitManager.GetUnitsOfFaction(m_targetFaction);
                mostHealth.OrderByDescending(u => u.CurrentHealth);
                dest = mostHealth[0].Square;
                break;

            case DestinationPriorityMode.LeastHealth:
                List<Unit> leastHealth = board.UnitManager.GetUnitsOfFaction(m_targetFaction);
                leastHealth.OrderBy(u => u.CurrentHealth);
                dest = leastHealth[0].Square;
                break;
        }

        Path_AStar<BoardSquare> path = 
            board.GetPath(
                unit.Square,
                unit.Proto.Flying,
                (s) => {
                    float distance = Vector2Int.Distance(s.Position, dest.Position);
                    return distance >= m_minDistance && distance <= m_maxDistance; },
                (s) => { return Vector2Int.Distance(s.Position, dest.Position); }
            );

        return MoveAlongPath(path, unit, board);
        //if (path == null || path.Length() == 0)
        //{
        //    return new EmptyContext();
        //}

        //BoardSquare square = null;
        //BoardSquare last = null;
        //float dist = 0f;
        //while(dist <= unit.Proto.MoveSpeed)
        //{
        //    last = square;
        //    square = path.Dequeue();
        //    dist += 1f;

        //    if(path.Length() == 0)
        //    {
        //        return new SingleBoardSquareContext(AbilityPriority, unit, square, board);
        //    }
        //}

        //// for flying units, an intermediate square on the path may be occupied. 
        //// If so, go back one square on the path. If that fails, movement fails.
        //if(square.Unit != null)
        //{
        //    if(last.Unit != null)
        //    {
        //        return new EmptyContext();
        //    }

        //    square = last;
        //}

        //return new SingleBoardSquareContext(AbilityPriority, unit, square, board);
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
        if (!(context is SingleBoardSquareContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        //SingleBoardSquareContext ctx = (SingleBoardSquareContext)context;
        ctx.Actor.FaceToward(ctx.Square);
        ctx.Board.TryMoveUnitTo(ctx.Actor, ctx.Square);
    }


}
