using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LifeStealAttack", menuName ="Units/Abilities/LifeStealAttack", order =116)]
public class LifeStealAttack : SingleTargetAttack
{
    [Range(0,1)] public float LifeStealPercent = 0.5f;

    //public override void Execute(IAbilityContext context)
    //{
    //    if (!(context is SingleTargetContext ctx))
    //    {
    //        Debug.LogError("Invalid context");
    //        return;
    //    }

    //    int damageDealt = ctx.Target.CurrentHealth;
    //    base.Execute(context);
    //    damageDealt -= ctx.Target.CurrentHealth;
    //    context.Actor.ChangeHealth(Mathf.RoundToInt(LifeStealPercent * damageDealt));
    //}

    public override AbilityInstance GetInstance()
    {
        return new LifeStealAttackInstance(this);
    }
}
