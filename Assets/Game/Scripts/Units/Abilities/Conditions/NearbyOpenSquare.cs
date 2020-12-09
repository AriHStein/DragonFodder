using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NearbyOpenSquare", menuName = "Units/Conditions/NearbyOpenSquare", order = 121)]
public class NearbyOpenSquare : Condition
{
    [SerializeField] float m_distance = 1f;
    public override bool IsMet(Unit unit, Board_Base board) 
    {
        // return true if at least one square exists without a unit in it within range
        foreach(BoardSquare square in board.GetSquaresInRange(unit.Square.Position, m_distance))
        {
            if(square.Unit == null)
            {
                return true;
            }
        }

        return true;
    }
}
