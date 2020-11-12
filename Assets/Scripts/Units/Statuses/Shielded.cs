using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Shielded", menuName = "Units/Statuses/Shielded", order = 126)]
public class Shielded : Status
{
	public override string Type { get { return "Shielded"; } }
    [SerializeField] int m_maxShieldAmount = 25;

    public override StatusInstance GetInstance()
    {
        return new ShieldInstance(this, m_maxShieldAmount);
    }
}

public class ShieldInstance : StatusInstance
{
    protected int m_shieldLeft;

    public ShieldInstance(Status prototype, int amount) : base(prototype)
    {
        m_shieldLeft = amount;
    }

    public override void CombineWith(StatusInstance other)
    {
        if(!(other is ShieldInstance si))
        {
            throw new ArgumentException($"Attempting to combine two different types of status. {Type} and {other.Type}");
        }
        
        base.CombineWith(si);
        m_shieldLeft += si.m_shieldLeft;
    }

    public override int ModifyDamageRecieved(int damage)
    {
        int reduction = Mathf.Min(damage, m_shieldLeft);
        damage -= reduction;
        m_shieldLeft -= reduction;

        if(m_shieldLeft <= 0)
        {
            Expire();
        }

        return damage;
    }
}
