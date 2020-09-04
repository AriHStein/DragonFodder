using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    [SerializeField] int m_damage = 1;
    
    public override void DoTurn()
    {
        Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        Unit target = Board.Current.UnitManager.GetNearestUnitOfFaction(targetFaction, Square);

        if (target == null)
        {
            Debug.LogError("No Target found");
            return;
        }

        if (Mathf.Abs(target.Square.Pos.x - Square.Pos.x) <= 1 && Mathf.Abs(target.Square.Pos.y - Square.Pos.y) <= 1)
        {
            FaceToward(target.Square);
            Attack(target);
        }
        else
        {
            MoveToward(target.Square);
        }
    }

    protected override void Attack(Unit target)
    {
        base.Attack(target);
        target.TakeDamage(m_damage);
    }
}
