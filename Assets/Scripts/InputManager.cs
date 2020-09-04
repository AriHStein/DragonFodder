using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Board m_board;
    Unit m_selectedUnit;
    [SerializeField] LayerMask m_boardLayer;
    [SerializeField] GameObject m_mouseIndicator = default;
    
    // Start is called before the first frame update
    void Start()
    {
        m_board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        m_mouseIndicator.SetActive(false);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TimeManager.TogglePause();
        }

        if (!TimeManager.Paused)
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
            m_selectedUnit = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (m_selectedUnit != null)
            {
                if (TryMoveSelectedToSquare(squareUnderMouse))
                {
                    return;
                }
            }
            else
            {
                if (squareUnderMouse.Unit != null)
                {
                    m_selectedUnit = squareUnderMouse.Unit;
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

    bool TryMoveSelectedToSquare(BoardSquare square)
    {
        if(square == null || m_selectedUnit == null || m_selectedUnit.Square == square)
        {
            return false;
        }

        if(m_board.TryMoveUnitTo(m_selectedUnit, square))
        {
            m_selectedUnit = null;
            return true;
        }

        return false;
    }
}
