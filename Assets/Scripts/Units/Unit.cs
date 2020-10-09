using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public enum Faction { Player, Enemy }
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(MaterialSwapper))]
public  abstract class Unit : MonoBehaviour
{

    [SerializeField] UnitPrototype m_prototype = default;
    public UnitPrototype Proto { 
        get { return m_prototype; } 
    }
    [HideInInspector]
    public BoardSquare Square;

    public string Type { get; protected set; }
    public Faction Faction { get; protected set; }
    public Guid ID { get; protected set; }
    public int MaxHealth { get { return m_prototype.MaxHealth; } }
    public int CurrentHealth { get; protected set; }

    public event Action<Unit> DeathEvent;

    protected Animator m_animator;
    protected MaterialSwapper m_materialSwapper;

    protected Path_AStar<BoardSquare> path;



    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_materialSwapper = GetComponent<MaterialSwapper>();
    }

    protected virtual void Start()
    {

        //CurrentHealth = MaxHealth;
    }

    public virtual void Initialize(BoardSquare square, UnitData data)
    {
        ID = data.ID;
        Type = data.Type;
        //Proto = Board.Current.UnitManager.GetUnitPrototypeOfType(data.Type);

        Square = square;
        Square.Unit = this;

        //MaxHealth = data.Proto.MaxHealth;
        //MaxHealth = Board.Current.UnitManager.GetUnitPrototypeOfType(data.Type).MaxHealth;
        if (data.CurrentHealth == -1)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth = data.CurrentHealth;
        }


        Faction = data.Faction;
        if(m_materialSwapper != null)
        {
            m_materialSwapper.SwapMaterial(Faction);
        }

        m_timeUntilNextTurn = Proto.TimeBetweenActions;

        Board.Current.UnitManager.RegisterUnit(this);
    }

    float m_timeUntilNextTurn;
    public virtual bool ReadyForTurn(float deltaTime)
    {
        m_timeUntilNextTurn -= deltaTime;
        if(m_timeUntilNextTurn <= 0)
        {
            m_timeUntilNextTurn = Proto.TimeBetweenActions + UnityEngine.Random.Range(-2f, 2)*deltaTime;
            return true;
        }

        return false;
    }
    public abstract void DoTurn();

    public void FaceToward(BoardSquare square)
    {
        transform.LookAt(square.transform);
    }

    protected virtual void Attack(Unit target)
    {
        m_animator.SetTrigger("Attack");
        target.ChangeHealth(-Proto.AttackDamage);
    }

    protected virtual bool MoveToward(BoardSquare dest)
    {
        Vector2Int offset = dest.Position - Square.Position;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            FaceToward(Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
            return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
        }
        else
        {
            FaceToward(Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
            return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
        }
    }

    protected virtual bool GetPathTo(BoardSquare dest)
    {
        path = Board.Current.GetPath(
                Square,
                Proto.Flying,
                (square) => { return Vector2Int.Distance(square.Position, dest.Position) <= 1; },
                (square) => { return Vector2Int.Distance(square.Position, dest.Position); }
            );

        if(path == null || path.Length() == 0)
        {
            return false;
        }

        path.Dequeue();
        return true;
    }

    public virtual void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }

    //public virtual void Heal(int amount)
    //{
    //    CurrentHealth += amount;
    //    CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    //}

    protected virtual void Die()
    {
        DeathEvent?.Invoke(this);
        //Square.Unit = null;
        //Destroy(gameObject);
    }
}
