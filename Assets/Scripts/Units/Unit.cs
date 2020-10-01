using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Faction { Player, Enemy }
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(MaterialSwapper))]
public  abstract class Unit : MonoBehaviour
{
    public string Type { get; protected set; }
    [SerializeField] UnitPrototype m_prototype = default;
    public UnitPrototype Proto { get { return m_prototype; } }
    [HideInInspector]
    public BoardSquare Square;
    public Faction Faction { get; protected set; }
    public Guid ID { get; protected set; }

    protected Animator m_animator;
    protected MaterialSwapper m_materialSwapper;

    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }

    public event Action<Unit> DeathEvent;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_materialSwapper = GetComponent<MaterialSwapper>();
    }

    protected virtual void Start()
    {

        //CurrentHealth = MaxHealth;
    }

    public void Initialize(BoardSquare square, UnitData data)
    {
        ID = data.ID;
        Type = data.Type;
        //Proto = data.Proto;

        Square = square;
        Square.Unit = this;

        //MaxHealth = data.Proto.MaxHealth;
        MaxHealth = Board.Current.UnitManager.GetUnitPrototypeOfType(data.Type).MaxHealth;
        CurrentHealth = data.CurrentHealth;

        Faction = data.Faction;
        if(m_materialSwapper != null)
        {
            m_materialSwapper.SwapMaterial(Faction);
        }

        Board.Current.UnitManager.RegisterUnit(this);
    }

    public abstract void DoTurn();

    public void FaceToward(BoardSquare square)
    {
        transform.LookAt(square.transform);
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
