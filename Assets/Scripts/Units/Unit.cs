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

    public int MaxHealth;
    public int CurrentHealth { get; protected set; }

    public event System.Action<Unit> DeathEvent;

    protected virtual void Start()
    {
        m_animator = GetComponent<Animator>();
        
        Board.Current.UnitManager.RegisterUnit(this);
        CurrentHealth = MaxHealth;
    }

    public void Initialize(BoardSquare square, UnitData data)
    {
        Square = square;
        Square.Unit = this;

        MaxHealth = data.MaxHealth;
        CurrentHealth = data.CurrentHealth;

        Faction = data.Faction;
    }

    public abstract void DoTurn();

    protected void FaceToward(BoardSquare square)
    {
        transform.rotation.SetLookRotation((Vector3Int)(square.Position - Square.Position));
    }

    protected virtual void Attack(Unit target)
    {
        m_animator.SetTrigger("Attack");
    }

    protected virtual void MoveToward(BoardSquare dest)
    {
        Vector2Int offset = dest.Position - Square.Position;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            FaceToward(Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
            Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
        }
        else
        {
            FaceToward(Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
            Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
        }
    }

    public virtual void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    protected virtual void Die()
    {
        DeathEvent?.Invoke(this);
        //Square.Unit = null;
        //Destroy(gameObject);
    }
}
