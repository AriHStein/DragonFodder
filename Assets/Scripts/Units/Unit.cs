﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Faction { Player, Enemy }
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MaterialSwapper))]
public  abstract class Unit : MonoBehaviour
{
    public string Type;
    public BoardSquare Square;
    public Faction Faction;
    public Guid ID { get; protected set; }

    protected Animator m_animator;
    protected MaterialSwapper m_materialSwapper;

    public int MaxHealth;
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

        Square = square;
        Square.Unit = this;

        MaxHealth = data.MaxHealth;
        CurrentHealth = data.CurrentHealth;

        Faction = data.Faction;
        m_materialSwapper.SwapMaterial(Faction);
        Board.Current.UnitManager.RegisterUnit(this);
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
