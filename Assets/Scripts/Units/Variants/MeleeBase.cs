using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBase : Unit
{
    //[SerializeField] int m_damage = 1;
    [SerializeField] GameObject m_stompParticleBurst = default;
    [SerializeField] float m_spellStunTime = 1f;

    public override void DoTurn()
    {
        Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        Unit target = Board.Current.UnitManager.GetNearestUnitOfFaction(targetFaction, Square);

        if (target == null)
        {
            Debug.LogWarning("No Target found");
            return;
        }

        if(CanCastSpell(null))
        {
            CastSpell();
            return;
        }
        else
        if (Mathf.Abs(target.Square.Position.x - Square.Position.x) <= 1 && Mathf.Abs(target.Square.Position.y - Square.Position.y) <= 1)
        {
            FaceToward(target.Square);
            Attack(target);
        }
        else
        {
            FaceToward(target.Square);
            if(!TryMoveToward(target.Square))
            {
                //Debug.Log("Look for a path.");
                if(TryGetPathTo(target.Square))
                {
                    //Debug.Log("Found a path");
                    TryMoveToward(path.Dequeue());
                }
            }
        }
    }

    protected override void CastSpell()
    {
        base.CastSpell();
        Instantiate(m_stompParticleBurst, transform);
        Vector2Int[] directions = new Vector2Int[8]
        {
            Vector2Int.up,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.left,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.right
        };

        foreach(Vector2Int direction in directions)
        {
            BoardSquare targetSquare = Board.Current.GetSquareAt(Square.Position + direction);
            if(targetSquare == null || targetSquare.Unit == null || targetSquare.Unit.Faction == Faction)
            {
                continue;
            }

            targetSquare.Unit.Stun(m_spellStunTime);
            targetSquare.Unit.TryForceMove(direction);
        }
    }

    //protected override void Attack(Unit target)
    //{
    //    base.Attack(target);
    //    target.TakeDamage(m_damage);
    //}
}
