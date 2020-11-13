using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealAttackInstance : SingleTargetAttackInstance
{
    float m_lifeStealPercent = 0.5f;

    public LifeStealAttackInstance(LifeStealAttack proto) : base(proto)
    {
        m_lifeStealPercent = proto.LifeStealPercent;
    }

    public override void Execute(IAbilityContext context)
    {
        if (!(context is SingleTargetContext ctx))
        {
            Debug.LogError("Invalid context");
            return;
        }

        int damageDealt = ctx.Target.CurrentHealth;
        base.Execute(context);
        damageDealt -= ctx.Target.CurrentHealth;
        context.Actor.ChangeHealth(Mathf.RoundToInt(m_lifeStealPercent * damageDealt));
    }
}
