using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAll : MonoBehaviour
{
    //[SerializeField] float m_stunTime = 1f;
    [SerializeField] Stun m_stun = default;
    public void StunAllUnits()
    {
        if(Board.Current == null || 
            Board.Current.UnitManager == null || 
            Board.Current.UnitManager.Units == null || 
            Board.Current.UnitManager.Units.Count == 0)
        {
            return;
        }
        
        foreach(Unit unit in Board.Current.UnitManager.Units)
        {
            //unit.Stun(m_stunTime);
            unit.ApplyStatus(m_stun.GetInstance());
        }
    }
}
