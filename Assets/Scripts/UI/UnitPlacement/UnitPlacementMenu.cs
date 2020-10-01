using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementMenu : MonoBehaviour
{
    [SerializeField] GameObject m_unitPanelPrefab = default;
    [SerializeField] Transform m_buttonsPanelTransform = default;
    Dictionary<string, PlaceUnitPanel> m_unitPanels;
    
    // Start is called before the first frame update
    void Awake()
    {
        m_unitPanels = new Dictionary<string, PlaceUnitPanel>();
    }

    public void Activate(List<UnitData> availableUnits)
    {
        gameObject.SetActive(true);
        foreach (PlaceUnitPanel panel in m_unitPanels.Values)
        {
            panel.Deactivate();
        }
        //if (m_unitPanels.Count > 0)
        //{

        //}


        if(availableUnits == null || availableUnits.Count == 0)
        {
            return;
        }
        
        Dictionary<string, List<UnitData>> sortedUnits = new Dictionary<string, List<UnitData>>();
        foreach(UnitData unit in availableUnits)
        {
            if(!sortedUnits.ContainsKey(unit.Type))
            {
                sortedUnits[unit.Type] = new List<UnitData>();
            }

            sortedUnits[unit.Type].Add(unit);
        }

        SetupPanels(sortedUnits);
    }

    public void Deactivate()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        
        foreach(PlaceUnitPanel panel in m_unitPanels.Values)
        {
            panel.Deactivate();
        }

        gameObject.SetActive(false);
    }

    void SetupPanels(Dictionary<string, List<UnitData>> sortedUnits)
    {
        foreach(string type in sortedUnits.Keys)
        {
            SetupPanel(type, sortedUnits[type]);
            
            //if (!m_unitPanels.ContainsKey(type))
            //{
            //    GameObject go = Instantiate(m_unitPanelPrefab, m_buttonsPanelTransform);
            //    go.transform.SetAsFirstSibling();
            //    m_unitPanels[type] = go.GetComponent<PlaceUnitPanel>();
            //}

            //m_unitPanels[type].UpdatePanel(sortedUnits[type]);
        }

    }

    void SetupPanel(string type, List<UnitData> units)
    {
        if (!m_unitPanels.ContainsKey(type))
        {
            GameObject go = Instantiate(m_unitPanelPrefab, m_buttonsPanelTransform);
            go.transform.SetAsFirstSibling();
            m_unitPanels[type] = go.GetComponent<PlaceUnitPanel>();
        }

        m_unitPanels[type].UpdatePanel(units);
    }

    public void AddUnit(UnitPrototype proto)
    {
        UnitData unit = new UnitData(proto, Vector2Int.zero, Faction.Player);
        if (!m_unitPanels.ContainsKey(proto.Type))
        {
            List<UnitData> units = new List<UnitData> { unit };
            SetupPanel(proto.Type, units);
        } else
        {
            m_unitPanels[proto.Type].AddUnit(unit);
        }
    }
}
