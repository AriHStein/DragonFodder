using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAll : MonoBehaviour
{
    //[SerializeField] float m_stunTime = 1f;
    [SerializeField] Stun m_stun = default;
    Board_Base m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board_Base>();
    }

    public void StunAllUnits()
    {
        //if(Board.Current == null || 
        //    Board.Current.UnitManager == null || 
        //    Board.Current.UnitManager.Units == null || 
        //    Board.Current.UnitManager.Units.Count == 0)
        //{
        //    return;
        //}
        
        foreach(Unit unit in m_board.UnitManager.Units)
        {
            //unit.Stun(m_stunTime);
            unit.ApplyStatus(m_stun.GetInstance());
        }
    }
}
