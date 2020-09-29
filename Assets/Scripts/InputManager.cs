using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{

    [SerializeField] LayerMask m_boardLayer;
    [SerializeField] GameObject m_mouseIndicator = default;

    Unit m_grabbedUnit;
    UnitData m_newUnitToPlace;

    public event Action UnitPlacedEvent;
    public event Action CancelPlacementEvent;

    Faction m_currentFaction;

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
            CancelPlacementEvent?.Invoke();
            //m_grabbedUnit = null;
            m_newUnitToPlace = new UnitData();
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
            else if(m_newUnitToPlace.ID != Guid.Empty)
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

    public void SelectUnitToPlace(UnitData data, Faction faction)
    {
        m_newUnitToPlace = data;
        m_currentFaction = faction;
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
        if(square == null || m_newUnitToPlace.ID == Guid.Empty || square.Unit != null)
        {
            return false;
        }

        m_newUnitToPlace.Position = square.Position;
        m_newUnitToPlace.Faction = m_currentFaction;
        //Board.Current.TryPlaceUnit(new UnitData(m_newUnitToPlace, square.Position, m_currentFaction));
        if(Board.Current.TryPlaceUnit(m_newUnitToPlace))
        {
            UnitPlacedEvent?.Invoke();
            m_newUnitToPlace = new UnitData();
            return true;
        } else
        {
            return false;
        }
    }
}
