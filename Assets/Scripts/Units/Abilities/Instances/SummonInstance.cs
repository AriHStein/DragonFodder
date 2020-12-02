using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonInstance : AbilityInstance
{
    UnitPrototype m_summonPrototype;
    
    public SummonInstance(Summon proto) : base(proto)
    {
        m_summonPrototype = proto.SummonPrototype;
    }

    protected override IAbilityContext GetValueOverride(Unit unit, Board board)
    {
        BoardSquare target = null;
        foreach (BoardSquare square in board.GetSquaresInRange(unit.Square.Position, 1f))
        {
            if (square.Unit == null)
            {
                target = square;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogError("No available square found. This should have been caught by the open square condition.");
            return new EmptyContext();
        }

        return new SingleBoardSquareContext(m_abilityPriority, unit, target, board);
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
