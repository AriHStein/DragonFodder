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

    public void Activate(List<UnitSerializationData> availableUnits)
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
        
        Dictionary<string, List<UnitSerializationData>> sortedUnits = new Dictionary<string, List<UnitSerializationData>>();
        foreach(UnitSerializationData unit in availableUnits)
        {
            if(!sortedUnits.ContainsKey(unit.Type))
            {
                sortedUnits[unit.Type] = new List<UnitSerializationData>();
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

    void SetupPanels(Dictionary<string, List<UnitSerializationData>> sortedUnits)
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

    void SetupPanel(string type, List<UnitSerializationData> units)
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
        UnitSerializationData unit = new UnitSerializationData(proto, Vector2Int.zero, Faction.Player);
        if (!m_unitPanels.ContainsKey(proto.Type))
        {
            List<UnitSerializationData> units = new List<UnitSerializationData> { unit };
            SetupPanel(proto.Type, units);
        } else
        {
            m_unitPanels[proto.Type].AddUnit(unit);
        }
    }
}
