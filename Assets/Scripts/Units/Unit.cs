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

    //[SerializeField] UnitPrototype m_prototype = default;
    //public UnitPrototype Proto { 
    //    get { return m_prototype; } 
    //}
    [HideInInspector]
    public BoardSquare Square;

    public string Type { get; protected set; }
    public Faction Faction { get; protected set; }
    public Guid ID { get; protected set; }
    public int Difficulty { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }

    public int MaxMP { get; protected set; }
    public int CurrentMP { get; protected set; }

    public float MoveSpeed { get; protected set; }
    public bool Flying { get; protected set; }
    public bool IsSummoned { get; protected set; }

    float m_timeBetweenActions;
    float m_timeSinceLastAction;
    float m_timeSinceLastManaAdd;

    protected List<AbilityInstance> m_abilities;
    protected List<StatusInstance> m_statuses;

    public event Action<Unit> DeathEvent;
    public event Action<Unit> InitializedEvent;

    protected Animator m_animator;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_statuses = new List<StatusInstance>();
        m_abilities = new List<AbilityInstance>();
    }

    public virtual void Initialize(Board board, BoardSquare square, UnitSerializationData data, UnitPrototype proto)
    {
        ID = data.ID;
        Type = proto.Type;
        Difficulty = proto.Difficulty;
        MaxHealth = proto.MaxHealth;
        MaxMP = proto.MaxMP;
        MoveSpeed = proto.MoveSpeed;
        Flying = proto.Flying;
        m_timeBetweenActions = proto.TimeBetweenActions;

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

        foreach(AbilityPrototype aProto in proto.BaseAbilities)
        {
            m_abilities.Add(aProto.GetInstance());
        }


        Faction = data.Faction;
        IsSummoned = data.IsSummoned;

        m_timeSinceLastAction = 0;

        board.UnitManager.RegisterUnit(this);

        InitializedEvent?.Invoke(this);
    }


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
        if(m_timeSinceLastAction >= m_timeBetweenActions)
        {
            m_timeSinceLastAction = 0;
            return true;
        }

        return false;
    }

    public virtual void DoTurn(Board board)
    {
        AbilityInstance bestAbility = null;
        IAbilityContext bestContext = null;
        foreach(AbilityInstance ability in m_abilities)
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

    //public virtual bool TryForceMove(Vector2Int moveVector)
    //{
    //    return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position + moveVector));
    //}

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
        foreach(AbilityInstance ability in m_abilities)
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
        foreach(AbilityInstance ability in m_abilities)
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
        foreach(StatusInstance status in new List<StatusInstance>(m_statuses))
        {
            amount = status.ModifyHealthChange(amount);
        }
        
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
