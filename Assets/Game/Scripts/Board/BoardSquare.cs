using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public Unit Unit;
    public Vector2Int Position;

    public bool Interactable = true;

    //public void Clear()
    //{
    //    if(Unit != null)
    //    {
    //        Board.Current.UnitManager.RemoveUnit(Unit);
    //    }
    //}
}
