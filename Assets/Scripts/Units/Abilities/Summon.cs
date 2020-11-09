using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon", menuName = "Units/Abilities/Summon", order = 116)]
public class Summon : Ability
{
    public override string AnimationTrigger { get { return null; } }

    [SerializeField] UnitPrototype m_summonPrototype = default;
    
    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if (result != null)
        {
            return result;
        }

        BoardSquare target = null;
        foreach(BoardSquare square in board.GetSquaresInRange(unit.Square.Position, 1f))
        {
            if(square.Unit == null)
            {
                target = square;
                break;
            }
        }

        if(target == null)
        {
            Debug.LogError("No available square found. This should have been caught by the open square condition.");
            return new EmptyContext();
        }

        return new SingleBoardSquareContext(AbilityPriority, unit, target, board);
    }
    
    public override void Execute(IAbilityContext context)
    {
        if (!(context is SingleBoardSquareContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        ctx.Actor.FaceToward(ctx.Square);
        ctx.Board.TryPlaceUnit(new UnitData(m_summonPrototype, ctx.Square.Position, ctx.Actor.Faction, true));

        base.Execute(context);
    }
    
    public override bool CanTargetUnit(Unit unit, Unit other)
    {
        return false;
    }
    
    public override bool CanTargetSquare(Unit unit, BoardSquare target)
    {
        return target.Unit == null && (unit.Square.Position - target.Position).sqrMagnitude == 1;
    }
}
