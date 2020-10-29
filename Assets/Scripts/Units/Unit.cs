using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public enum Faction { Player, Enemy }
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
    //protected int SpellMP { get { return Proto.SpellMP; } }

    protected List<StatusInstance> m_statuses;
    //[SerializeField] protected List<Ability> m_abilities;

    public event Action<Unit> DeathEvent;
    public event Action<Unit> InitializedEvent;

    protected Animator m_animator;
    //[SerializeField] GameObject m_stunnedParticles = default;

    //protected Path_AStar<BoardSquare> path;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        //m_stunnedParticles.SetActive(false);
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

        if (data.CurrentMP == -1)
        {
            CurrentMP = 0;
        }
        else
        {
            CurrentMP = data.CurrentMP;
        }


        Faction = data.Faction;

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
        
        //if(m_stunnedTimeLeft > 0)
        //{
        //    m_stunnedTimeLeft -= deltaTime;
        //    return false;
        //}

        //m_animator.SetBool("Stunned", false);
        //m_stunnedParticles.SetActive(false);

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
            if(bestContext == null || context.Value > bestContext.Value)
            {
                bestAbility = ability;
                bestContext = context;
            }
        }

        bestAbility.Execute(bestContext);
        if(bestAbility.AnimationTrigger != null)
        {
            m_animator.SetTrigger(bestAbility.AnimationTrigger);
        }
    }

    //protected virtual bool CanCastSpell(Unit target)
    //{
    //    return CurrentMP >= SpellMP;
    //}

    //protected virtual void CastSpell()
    //{
    //    //if(!CanCastSpell())
    //    //{
    //    //    return false;
    //    //}

    //    ChangeMP(-SpellMP);
    //    //return true;
    //}

    public void FaceToward(BoardSquare square)
    {
        transform.LookAt(square.transform);
    }

    //protected virtual void Attack(Unit target)
    //{
    //    m_animator.SetTrigger("Attack");
    //    target.ChangeHealth(-Proto.AttackDamage);
    //}

    //protected virtual bool TryMoveToward(BoardSquare dest)
    //{
    //    Vector2Int offset = dest.Position - Square.Position;
    //    if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
    //    {
    //        FaceToward(Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
    //        return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x + (int)Mathf.Sign(offset.x), Square.Position.y));
    //    }
    //    else
    //    {
    //        FaceToward(Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
    //        return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position.x, Square.Position.y + (int)Mathf.Sign(offset.y)));
    //    }
    //}

    public virtual bool TryForceMove(Vector2Int moveVector)
    {
        return Board.Current.TryMoveUnitTo(this, Board.Current.GetSquareAt(Square.Position + moveVector));
    }

    //protected virtual bool TryGetPathTo(BoardSquare dest)
    //{
    //    path = Board.Current.GetPath(
    //            Square,
    //            Proto.Flying,
    //            (square) => { return Vector2Int.Distance(square.Position, dest.Position) <= 1; },
    //            (square) => { return Vector2Int.Distance(square.Position, dest.Position); }
    //        );

    //    if(path == null || path.Length() == 0)
    //    {
    //        return false;
    //    }

    //    path.Dequeue();
    //    return true;
    //}

    //float m_stunnedTimeLeft = 0f;
    //public void Stun(float time)
    //{
    //    ApplyStatus(new Stun(time));
    //    //m_stunnedTimeLeft += time;
    //    m_stunnedParticles.SetActive(true);
    //    //m_animator.SetBool("Stunned", true);
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

    protected virtual void ChangeMP(int amount)
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

        //Square.Unit = null;
        //Destroy(gameObject);
    }

    public void VictoryDance()
    {
        m_animator.SetTrigger("Victory");
    }
}
