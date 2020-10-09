using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBase : Unit
{
    //[SerializeField] int m_damage = 1;
    
    public override void DoTurn()
    {
        Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        Unit target = Board.Current.UnitManager.GetNearestUnitOfFaction(targetFaction, Square);

        if (target == null)
        {
            Debug.LogWarning("No Target found");
            return;
        }

        if (Mathf.Abs(target.Square.Position.x - Square.Position.x) <= 1 && Mathf.Abs(target.Square.Position.y - Square.Position.y) <= 1)
        {
            FaceToward(target.Square);
            Attack(target);
        }
        else
        {
            FaceToward(target.Square);
            if(!MoveToward(target.Square))
            {
                //Debug.Log("Look for a path.");
                if(GetPathTo(target.Square))
                {
                    //Debug.Log("Found a path");
                    MoveToward(path.Dequeue());
                }
            }
        }
    }

    //protected override void Attack(Unit target)
    //{
    //    base.Attack(target);
    //    target.TakeDamage(m_damage);
    //}
}
