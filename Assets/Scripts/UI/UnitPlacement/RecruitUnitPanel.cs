using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecruitUnitPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_costText = default;
    [SerializeField] TextMeshProUGUI m_buttonText = default;

    [SerializeField] int m_cost = 1;
    [SerializeField] UnitPrototype m_unit = default;

    UnitPlacementMenu m_placementMenu;
    Board m_board;

    private void Start()
    {
        m_placementMenu = FindObjectOfType<UnitPlacementMenu>();
        m_board = FindObjectOfType<Board>();

        m_costText.text = m_cost.ToString();
        m_buttonText.text = "Recruit " + m_unit.Type;
    }

    public void RecruitUnit()
    {
        if (m_board.ChangeGold(-m_cost))
        {
            m_placementMenu.AddUnit(m_unit);
        }
    }
}
