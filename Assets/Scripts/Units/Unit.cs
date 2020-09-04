using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { Player, Enemy }
[RequireComponent(typeof(Animator))]
public  abstract class Unit : MonoBehaviour
{
    public BoardSquare Square;
    public Faction Faction;

    protected Animator m_animator;

    [SerializeField] int m_maxHealth = 5;
    protected int m_currentHealth;

    public event System.Action<Unit> DeathEvent;

    protected virtual void Start()
    {
        m_animator = GetComponent<Animator>();
        
        Board.Current.UnitManager.RegisterUnit(this);
        m_currentHealth = m_maxHealth;
    }

    public abstract void DoTurn();

    protected void FaceToward(BoardSquare square)
    {
        transform.rotation.SetLookRotation((Vector3Int)(square.Pos - Square.Pos));
    }

    protected virtual void Attack(Unit target)
    {
        m_animator.SetTrigger("Attack");
    }

    protected virtual void MoveToward(BoardSquare dest)
    {
        Vector2Int offset = dest.Pos - Square.Pos;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            FaceToward(Board.Current.GetSquareAt(Square.Pos.x + (int)Mathf.Sign(offset.x), Square.Pos.y));
            Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Pos.x + (int)Mathf.Sign(offset.x), Square.Pos.y));
        }
        else
        {
            FaceToward(Board.Current.GetSquareAt(Square.Pos.x, Square.Pos.y + (int)Mathf.Sign(offset.y)));
            Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Pos.x, Square.Pos.y + (int)Mathf.Sign(offset.y)));
        }
    }

    public virtual void TakeDamage(int amount)
    {
        m_currentHealth -= amount;
        if(m_currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        m_currentHealth += amount;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
    }

    protected virtual void Die()
    {
        DeathEvent?.Invoke(this);
        //Square.Unit = null;
        //Destroy(gameObject);
    }
}
