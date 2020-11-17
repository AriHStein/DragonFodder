using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaCondition", menuName = "Units/Conditions/ManaCondition", order = 121)]
public class ManaCondition : Condition
{
    [SerializeField] int m_minMana = 1;
    
    public override bool IsMet(Unit unit, Board board) 
    {
        return unit.CurrentMP >= m_minMana;
    }
}
