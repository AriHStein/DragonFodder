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

    Board m_board;

    private void Start()
    {
        m_board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        CleanupPreviews();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            TimeManager.TogglePause();
        }

        if (m_board.PlayMode == PlayMode.Battle)
        {
            return;
        }

        HandleSelectionInput();
    }

    void CleanupPreviews()
    {
        m_mouseIndicator.SetActive(false);

        if (m_unitPreview != null)
        {
            m_unitPreview.SetActive(false);
        }
    }

    void HandleSelectionInput()
    {

        if (Input.GetMouseButtonDown(1))
        {
            ReturnGrabbedUnitToOriginalPosition();
            CancelPlacementEvent?.Invoke();
            //m_grabbedUnit = null;
            //m_newUnitToPlace = new UnitData();
            m_newUnitToPlace = null;
        }

        BoardSquare squareUnderMouse = GetSquareUnderMouse();
        if (squareUnderMouse == null || !squareUnderMouse.Interactable)
        {
            return;
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
            //else if(m_newUnitToPlace.ID != Guid.Empty)
            else if(m_newUnitToPlace != null)
            {
                if(TryPlaceNewUnit(squareUnderMouse))
                {
                    return;
                }
            }
            else
            {
                if(GrabUnitAt(squareUnderMouse))
                {
                    return;
                }
            }
        }
        else
        {
            ShowPreviewAt(squareUnderMouse);
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

    bool TryMoveUnitSelectedToSquare(BoardSquare square)
    {
        if (square == null || m_grabbedUnit == null || m_grabbedUnit.Square == square)
        {
            return false;
        }

        if (m_board.TryMoveUnitTo(m_grabbedUnit, square))
        {
            m_grabbedUnit.gameObject.SetActive(true);
            m_grabbedUnit = null;
            ClearPreviewUnit();
            return true;
        }

        return false;
    }

    bool TryPlaceNewUnit(BoardSquare square)
    {
        if (square == null || 
            //m_newUnitToPlace.ID == Guid.Empty || 
            m_newUnitToPlace == null ||
            square.Unit != null)
        {
            return false;
        }

        //m_newUnitToPlace.Position = square.Position;
        m_newUnitToPlace.Faction = m_currentFaction;
        //Board.Current.TryPlaceUnit(new UnitData(m_newUnitToPlace, square.Position, m_currentFaction));
        if (m_board.TryPlaceUnit(m_newUnitToPlace, square.Position) != null)
        {
            UnitPlacedEvent?.Invoke();
            //m_newUnitToPlace = new UnitData();
            m_newUnitToPlace = null;
            ClearPreviewUnit();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool GrabUnitAt(BoardSquare square)
    {
        if (square.Unit != null)
        {
            m_grabbedUnit = square.Unit;
            m_grabbedUnit.gameObject.SetActive(false);
            m_unitPreview = Instantiate(m_grabbedUnit.gameObject, transform);
            return true;
        }

        return false;
    }

    void ReturnGrabbedUnitToOriginalPosition()
    {
        if (m_grabbedUnit == null)
        {
            return;
        }

        m_grabbedUnit.gameObject.SetActive(true);
        m_grabbedUnit = null;
    }

    void ShowPreviewAt(BoardSquare square)
    {
        if (m_unitPreview != null)
        {
            m_unitPreview.transform.position = square.transform.position;
            m_unitPreview.gameObject.SetActive(true);
        }
        else
        {
            m_mouseIndicator.transform.position = square.transform.position + Vector3.up;
            m_mouseIndicator.SetActive(true);
        }
    }

    void ClearPreviewUnit()
    {
        if (m_unitPreview == null)
        {
            return;
        }

        Destroy(m_unitPreview);
        m_unitPreview = null;
    }


    public void SelectUnitToPlace(UnitData data, Faction faction)
    {
        ReturnGrabbedUnitToOriginalPosition();
        CancelPlacementEvent?.Invoke();

        m_newUnitToPlace = data;
        m_currentFaction = faction;
        //m_unitPreview = Instantiate(Board.Current.UnitManager.GetPrefabOfType(data.Type), transform);
        //m_unitPreview = Instantiate(m_board.UnitManager.GetUnitPrototypeOfType(data.Type).Prefab, transform);
        m_unitPreview = Instantiate(UnitPrototypeLookup.GetProto(data.Type).Prefab, transform);

    }
}
