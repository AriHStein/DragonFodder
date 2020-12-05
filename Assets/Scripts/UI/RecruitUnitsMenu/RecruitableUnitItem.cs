using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitableUnitItem : MonoBehaviour
{
    public UnitPrototype Unit { get; protected set; }
    [SerializeField] TextMeshProUGUI m_unitName = default;
    [SerializeField] Image m_portrait = default;
    [SerializeField] TextMeshProUGUI m_levelText = default;
    [SerializeField] TextMeshProUGUI m_costText = default;

    RecruitUnitsPanel m_recruitPanel;
    
    public void Initialize(UnitPrototype unit, RecruitUnitsPanel panel)
    {
        Unit = unit;
        m_recruitPanel = panel;

        m_unitName.text = Unit.Type;
        if(Unit.Portrait != null)
        {
            m_portrait.sprite = Unit.Portrait;
        }

        m_levelText.text = "Level 1";
        m_costText.text = Unit.RecruitCost.ToString() + " Gold";
    }

    public void RecruitUnit()
    {
        m_recruitPanel.RecruitUnit(this);
    }
}
