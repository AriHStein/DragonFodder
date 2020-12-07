using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invisible", menuName = "Units/Statuses/Invisible", order = 126)]
public class Invisible : Status
{
	public override string Type { get { return "Invisible"; } }

    public override bool IsTargetable()
    {
        return false;
    }

    public override StatusInstance GetInstance()
    {
        return new InvisibleInstance(this);
    }
}

public class InvisibleInstance : StatusInstance
{
    public InvisibleInstance(Status prototype) : base(prototype) { }

    public override int ModifyDamageDealt(int damage)
    {
        Expire();
        return base.ModifyDamageDealt(damage);
    }
}
