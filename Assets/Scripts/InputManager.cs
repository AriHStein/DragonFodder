using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField] LayerMask m_boardLayer;
    [SerializeField] GameObject m_mouseIndicator = default;

    Unit m_grabbedUnit;
    UnitPrototype m_newUnitToPlace;

    GameObject m_unitPreview;

    // Update is called once per frame
    void Update()
    {
        m_mouseIndicator.SetActive(false);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TimeManager.TogglePause();
        }

        if (Board.Current.PlayMode == PlayMode.Battle)
        {
            return;
        }

        HandleSelectionInput();
    }

    void HandleSelectionInput()
    {

        BoardSquare squareUnderMouse = GetSquareUnderMouse();
        if (squareUnderMouse == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            ReturnGrabbedUnitToOriginalPosition();
            //m_grabbedUnit = null;
            m_newUnitToPlace = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (m_grabbedUnit != null)
            {
                if (TryMoveUnitSelectedToSquare(squareUnderMouse))
                {
                    return;
                }
            }
            else if(m_newUnitToPlace != null)
            {
                if(TryPlaceNewUnit(squareUnderMouse))
                {
                    return;
                }
            }
            else
            {
                if (squareUnderMouse.Unit != null)
                {
                    m_grabbedUnit = squareUnderMouse.Unit;
                    return;
                }
            }
        }
        else
        {
            m_mouseIndicator.transform.position = squareUnderMouse.transform.position + Vector3.up;
            m_mouseIndicator.SetActive(true);
        }
    }

    BoardSquare GetSquareUnderMouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return hit.collider.GetComponent<BoardSquare>();
        }

        return null;
    }

    public void SelectUnitToPlace(UnitPrototype data)
    {
        m_newUnitToPlace = data;
        ReturnGrabbedUnitToOriginalPosition();
    }

    void ReturnGrabbedUnitToOriginalPosition()
    {
        if(m_grabbedUnit == null)
        {
            return;
        }
        
        m_grabbedUnit = null;
    }

    bool TryMoveUnitSelectedToSquare(BoardSquare square)
    {
        if(square == null || m_grabbedUnit == null || m_grabbedUnit.Square == square)
        {
            return false;
        }

        if(Board.Current.TryMoveUnitTo(m_grabbedUnit, square))
        {
            m_grabbedUnit = null;
            return true;
        }

        return false;
    }

    bool TryPlaceNewUnit(BoardSquare square)
    {
        if(square == null || m_newUnitToPlace == null || square.Unit != null)
        {
            return false;
        }

        Board.Current.TryPlaceNewUnit(new UnitData(m_newUnitToPlace, square.Position));
        m_newUnitToPlace = null;
        return true;
    }
}
