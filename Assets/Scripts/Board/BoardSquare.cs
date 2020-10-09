using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public Unit Unit;
    public Vector2Int Position;

    public bool Interactable = true;

    //private void Start()
    //{
    //    if(Unit != null)
    //    {
    //        Unit.Square = this;
    //    }
    //}

    //public void Activate()
    //{
    //    if(Unit != null)
    //    {
    //        Unit.gameObject.SetActive(true);
    //    }

    //    gameObject.SetActive(true);
    //    Interactable = true;
    //}

    //public void Deactivate()
    //{
    //    if (Unit != null)
    //    {
    //        Unit.gameObject.SetActive(false);
    //    }

    //    gameObject.SetActive(false);
    //}

    public void Clear()
    {
        if(Unit != null)
        {
            Board.Current.UnitManager.RemoveUnit(Unit);
            //Destroy(Unit.gameObject);
            //Unit = null;
        }
    }
}
