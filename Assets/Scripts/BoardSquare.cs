using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public Unit Unit;
    public Vector2Int Pos;

    private void Start()
    {
        if(Unit != null)
        {
            Unit.Square = this;
        }
    }
}
