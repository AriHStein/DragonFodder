//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SlowMelee : MeleeBase
//{
//    [SerializeField] int m_numberOfTurnsToSkip = 1;

//    int m_skippedTurnCount;

//    protected override void Start()
//    {
//        m_skippedTurnCount = 0;
//        base.Start();
//    }

//    public override void DoTurn()
//    {
//        if(m_skippedTurnCount < m_numberOfTurnsToSkip)
//        {
//            m_skippedTurnCount++;
//            return;
//        }

//        m_skippedTurnCount = 0;
//        base.DoTurn();
//    }
//}
