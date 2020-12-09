using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecruitUnitPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_costText = default;
    [SerializeField] TextMeshProUGUI m_availableUnitsText = default;
    [SerializeField] TextMeshProUGUI m_buttonText = default;

    //[SerializeField] int m_cost = 1;
    UnitPrototype m_unit;
    int m_availableUnits;

    UnitPlacementMenu m_placementMenu;
    Board m_board;

    public void RecruitUnit()
    {
        //if (m_board.ChangeGold(-m_cost))
        //{
        //    m_placementMenu.AddUnit(m_unit);
        //}

        if (m_availableUnits <= 0)
        {
            Deactivate();
            return;
        }
        
        if (m_board.ChangeGold(-m_unit.RecruitCost))
        {
            m_placementMenu.AddUnitToPlace(m_unit);
            m_availableUnits--;
        }

        if (m_availableUnits <= 0)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        m_unit = null;
        m_availableUnits = 0;
    }

    public void Activate(UnitPrototype proto, int count, UnitPlacementMenu menu, Board board)
    {
        if(count <= 0 || proto == null)
        {
            return;
        }

        m_placementMenu = menu;
        m_board = board;
        m_unit = proto;
        m_availableUnits = count;
        m_costText.text = m_unit.RecruitCost.ToString() + " Gold";
        m_buttonText.text = "Recruit " + m_unit.Type;
        m_availableUnitsText.text = "Count: " + m_availableUnits.ToString();
    }
}
