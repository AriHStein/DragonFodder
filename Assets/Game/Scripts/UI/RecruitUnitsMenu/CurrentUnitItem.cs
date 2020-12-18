using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentUnitItem : MonoBehaviour
{
    public UnitData Unit { get; protected set; }
    [SerializeField] TextMeshProUGUI m_unitName = default;
    [SerializeField] Image m_portrait = default;
    [SerializeField] TextMeshProUGUI m_levelText = default;

    RecruitUnitsPanel m_recruitPanel;

    public void Initialize(UnitData unit, RecruitUnitsPanel panel)
    {
        Unit = unit;
        m_recruitPanel = panel;

        m_unitName.text = Unit.Type;

        UnitPrototype proto = UnitPrototypeDB.GetProto(unit.Type);
        if (proto.Portrait != null)
        {
            m_portrait.sprite = proto.Portrait;
        }

        m_levelText.text = "Level 1";
    }
}
