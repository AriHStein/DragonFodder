using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementMenu : MonoBehaviour
{
    [SerializeField] GameObject m_placeUnitPanelPrefab = default;
    [SerializeField] Transform m_buttonsParentTransform = default;
    [SerializeField] Transform m_placeUnitButtonsRoot = default;

    Dictionary<string, PlaceUnitPanel> m_placeUnitPanels;
    
    // Start is called before the first frame update
    void Awake()
    {
        m_placeUnitPanels = new Dictionary<string, PlaceUnitPanel>();
    }

    public void Activate(
        List<UnitData> currentUnits)
    {
        gameObject.SetActive(true);
        ActivateCurrentUnitPanels(currentUnits);
    }

    void ActivateCurrentUnitPanels(List<UnitData> currentUnits)
    {
        foreach (PlaceUnitPanel panel in m_placeUnitPanels.Values)
        {
            panel.Deactivate();
        }

        if (currentUnits == null || currentUnits.Count == 0)
        {
            return;
        }

        Dictionary<string, List<UnitData>> sortedUnits = new Dictionary<string, List<UnitData>>();
        foreach (UnitData unit in currentUnits)
        {
            if (!sortedUnits.ContainsKey(unit.Type))
            {
                sortedUnits[unit.Type] = new List<UnitData>();
            }

            sortedUnits[unit.Type].Add(unit);
        }

        SetupPlaceUnitPanels(sortedUnits);

    }

    public void Deactivate()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        
        foreach(PlaceUnitPanel panel in m_placeUnitPanels.Values)
        {
            panel.Deactivate();
        }

        gameObject.SetActive(false);
    }

    void SetupPlaceUnitPanels(
        Dictionary<string, List<UnitData>> sortedUnits)
    {
        foreach(string type in sortedUnits.Keys)
        {
            SetupPlaceUnitPanel(type, sortedUnits[type]);
        }

    }

    void SetupPlaceUnitPanel(string type, List<UnitData> units)
    {
        if (!m_placeUnitPanels.ContainsKey(type))
        {
            GameObject go = Instantiate(m_placeUnitPanelPrefab, m_buttonsParentTransform);
            go.transform.SetSiblingIndex(m_placeUnitButtonsRoot.transform.GetSiblingIndex() + 1);
            m_placeUnitPanels[type] = go.GetComponent<PlaceUnitPanel>();
        }

        m_placeUnitPanels[type].UpdatePanel(units);
    }

    public List<UnitData> GetUnplacedUnits()
    {
        List<UnitData> units = new List<UnitData>();
        foreach(PlaceUnitPanel panel in m_placeUnitPanels.Values)
        {
            if(panel.AvailableUnits != null && panel.AvailableUnits.Count > 0)
            {
                units.AddRange(panel.AvailableUnits);
            }
        }

        return units;
    }

    public void AddUnitToPlace(UnitPrototype proto)
    {
        UnitData unit = new UnitData(
            proto, 
            Faction.Player);
        if (!m_placeUnitPanels.ContainsKey(proto.Type))
        {
            List<UnitData> units = new List<UnitData> { unit };
            SetupPlaceUnitPanel(proto.Type, units);
        }
        else
        {
            m_placeUnitPanels[proto.Type].AddUnit(unit);
        }
    }
}
