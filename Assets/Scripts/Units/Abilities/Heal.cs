using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Heal", menuName ="Units/Abilities/Heal", order =116)]
public class Heal : Ability
{
    public override string AnimationTrigger { get { return null; } }
    [SerializeField] float m_range = 1f;
    [SerializeField] int m_healAmount = 1;

    public override IAbilityContext GetValue(Unit unit, Board board)
    {
        IAbilityContext result = base.GetValue(unit, board);
        if (result != null)
        {
            return result;
        }

        return null;
    }
}
