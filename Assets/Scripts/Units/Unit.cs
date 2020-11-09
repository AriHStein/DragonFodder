using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public enum Faction { Player, Enemy }
public static class FactionExtensions 
{ 
    public static Faction Opposite(this Faction faction)
    {
        if(faction == Faction.Enemy)
        {
            return Faction.Player;
        }

        return Faction.Enemy;
    }
}

[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour
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

    public int MaxMP { get { return Proto.MaxMP; } }
    public int CurrentMP { get; protected set; }

    public bool IsSummoned { get; protected set; }

    protected List<StatusInstance> m_statuses;

    public event Action<Unit> DeathEvent;
    public event Action<Unit> InitializedEvent;

    protected Animator m_animator;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_statuses = new List<StatusInstance>();
    }

    protected virtual void Start()
    {

        //CurrentHealth = MaxHealth;
    }

    public virtual void Initialize(Board board, BoardSquare square, UnitData data)
    {
        ID = data.ID;
        Type = data.Type;

        Square = square;
        Square.Unit = this;

        if (data.CurrentHealth == -1)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth = data.CurrentHealth;
        }

        if (data.CurrentMP == -1)
        {
            CurrentMP = 0;
        }
        else
        {
            CurrentMP = data.CurrentMP;
        }


        Faction = data.Faction;
        IsSummoned = data.IsSummoned;

        m_timeSinceLastAction = 0;

        board.UnitManager.RegisterUnit(this);

        InitializedEvent?.Invoke(this);
    }

    float m_timeSinceLastAction;
    float m_timeSinceLastManaAdd;
    public virtual bool ReadyForTurn(float deltaTime)
    {
        bool ready = true;
        foreach(StatusInstance status in new List<StatusInstance>(m_statuses))
        {
            if(!status.ReadyForTurn(deltaTime))
            {
                ready = false;
            }
        }

        if(!ready)
        {
            return false;
        }

        m_timeSinceLastManaAdd += deltaTime;
        if(m_timeSinceLastManaAdd > 1)
        {
            ChangeMP(1);
            m_timeSinceLastManaAdd = 0;
        }


        m_timeSinceLastAction += deltaTime;
        if(m_timeSinceLastAction >= Proto.TimeBetweenActions)
        {
            m_timeSinceLastAction = 0;
            return true;
        }

        return false;
    }

    public virtual void DoTurn(Board board)
    {
        Ability bestAbility = null;
        IAbilityContext bestContext = null;
        foreach(Ability ability in Proto.Abilities)
        {
            IAbilityContext context = ability.GetValue(this, board);
            if(context.Value <= 0)
            {
                continue;
            }

            if(bestContext == null || context.Value > bestContext.Value)
            {
                bestAbility = ability;
                bestContext = context;
            }
        }

        // No valid actions found. skip turn
        if(bestAbility == null)
        {
            return;
        }

        bestAbility.Execute(bestContext);
        if(bestAbility.AnimationTrigger != null)
        {
            m_animator.SetTrigger(bestAbility.AnimationTrigger);
        }
    }

    public void FaceToward(BoardSquare square)
    {
        transform.LookAt(square.transform);
    }

    public virtual bool TryForceMove(Vector2Int moveVector)
    {
        return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position + moveVector));
    }

    public void ApplyStatus(StatusInstance status)
    {
        foreach(StatusInstance s in m_statuses)
        {
            if(s.Type == status.Type)
            {
                s.CombineWith(status);
                return;
            }
        }

        m_statuses.Add(status);
        if(status.Proto.EffectPrefab != null)
        {
            status.Effect = Instantiate(status.Proto.EffectPrefab, transform);
        }

        status.StatusExpiredEvent += OnStatusExpired;
    }

    void OnStatusExpired(StatusInstance status)
    {
        status.StatusExpiredEvent -= OnStatusExpired;

        if (!m_statuses.Contains(status))
        {
            Debug.LogWarning($"Status {status.Type} not contained in status list.");
            return;
        }

        m_statuses.Remove(status);
    }

    public bool IsTargetable()
    {
        foreach(StatusInstance status in m_statuses)
        {
            if(!status.IsTargetable())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanTargetUnit(Unit other)
    {
        foreach(Ability ability in Proto.Abilities)
        {
            if(ability.CanTargetUnit(this, other))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanTargetSquare(BoardSquare square)
    {
        foreach(Ability ability in Proto.Abilities)
        {
            if(ability.CanTargetSquare(this, square))
            {
                return true;
            }
        }

        return false;
    }

    public virtual void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        if(CurrentHealth <= 0)
        {
            Die();
        }

        if(amount < 0)
        {
            ChangeMP(-amount);
        }
    }

    public virtual void ChangeMP(int amount)
    {
        //Debug.Log("ChangeMP");
        CurrentMP += amount;
        CurrentMP = Mathf.Clamp(CurrentMP, 0, MaxMP);
    }

    protected virtual void Die()
    {
        foreach (StatusInstance status in m_statuses)
        {
            status.StatusExpiredEvent -= OnStatusExpired;
        }
        DeathEvent?.Invoke(this);
    }

    public void VictoryDance()
    {
        m_animator.SetTrigger("Victory");
    }
}
