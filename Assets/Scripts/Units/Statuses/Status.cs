using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Status : ScriptableObject
{
    public abstract string Type { get; }
    [SerializeField] float m_duration = default;
    public float Duration { get { return m_duration; } }
    [SerializeField] GameObject m_effectPrefab = default;
    public GameObject EffectPrefab { get { return m_effectPrefab; } }

    public virtual StatusInstance GetInstance()
    {
        return new StatusInstance(this);
    }

    public virtual bool ReadyForTurn(float timeLeft)
    {
        return true;
    }

    public virtual bool IsTargetable()
    {
        return true;
    }
    
    public virtual int ModifyDamageRecieved(int damage)
    {
        return damage;
    }

    public virtual int ModifyDamageDealt(int damage)
    {
        return damage;
    }
}

public class StatusInstance
{
    public StatusInstance(Status prototype)
    {
        Proto = prototype;
        m_timeLeft = prototype.Duration;
    }

    public string Type { get { return Proto.Type; } }
    public Status Proto { get; protected set; }
    protected float m_timeLeft;
    public event Action<StatusInstance> StatusExpiredEvent;
    public GameObject Effect;

    public virtual void CombineWith(StatusInstance other)
    {
        if (other == null || other.Type != Type)
        {
            throw new ArgumentException("Attempting to combine two different types of status.");
        }

        m_timeLeft += other.m_timeLeft;
    }

    public virtual bool ReadyForTurn(float deltaTime)
    {
        m_timeLeft -= deltaTime;
        if (m_timeLeft <= 0)
        {
            Expire();
        }
        return Proto.ReadyForTurn(m_timeLeft);
    }

    protected void Expire()
    {
        if(Effect != null)
        {
            GameObject.Destroy(Effect.gameObject);
        }
        StatusExpiredEvent?.Invoke(this);
    }

    public virtual bool IsTargetable()
    {
        return Proto.IsTargetable();
    }

    public virtual int ModifyDamageRecieved(int damage)
    {
        return Proto.ModifyDamageRecieved(damage);
    }

    public virtual int ModifyDamageDealt(int damage)
    {
        return Proto.ModifyDamageDealt(damage);
    }
}
