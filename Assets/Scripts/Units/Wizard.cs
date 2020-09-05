using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    [SerializeField] GameObject m_attackPrefab = default;
    [SerializeField] float m_range = 4f;


    public override void DoTurn()
    {
        Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        Unit target = Board.Current.UnitManager.GetNearestUnitOfFaction(targetFaction, Square);

        if(target == null)
        {
            Debug.LogWarning("No Target found");
            return;
        }

        if(Vector2Int.Distance(target.Square.Position, Square.Position) < m_range)
        {
            FaceToward(target.Square);
            Attack(target);
        } else
        {
            MoveToward(target.Square);
        }
    }

    protected override void Attack(Unit target)
    {
        if(target == null)
        {
            return;
        }

        base.Attack(target);
        GameObject fireball = Instantiate(m_attackPrefab, transform.position + Vector3.up, Quaternion.identity);
        fireball.GetComponent<Projectile>().Initialize(target);
    }

    //public override void TakeDamage(int amount)
    //{
    //    base.TakeDamage(amount);
    //    Debug.Log($"Take {amount} damage. Remaining health: {m_currentHealth}");
    //}


    //protected override void Die()
    //{
    //    base.Die();
    //    Debug.Log("Die");
    //}
}
