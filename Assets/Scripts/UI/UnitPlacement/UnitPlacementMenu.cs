using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementMenu : MonoBehaviour
{
    [SerializeField] GameObject m_placeUnitPanelPrefab = default;
    [SerializeField] GameObject m_recruitUnitPanelPrefab = default;
    [SerializeField] Transform m_buttonsParentTransform = default;
    [SerializeField] Transform m_placeUnitButtonsRoot = default;
    [SerializeField] Transform m_recruitUnitButtonsRoot = default;

    Dictionary<string, PlaceUnitPanel> m_placeUnitPanels;
    Dictionary<string, RecruitUnitPanel> m_recruitUnitPanels;
    
    // Start is called before the first frame update
    void Awake()
    {
        m_placeUnitPanels = new Dictionary<string, PlaceUnitPanel>();
        m_recruitUnitPanels = new Dictionary<string, RecruitUnitPanel>();

    }

    public void Activate(List<UnitData> currentUnits, List<RecruitableUnit> recruitableUnits, Board board)
    {
        gameObject.SetActive(true);
        ActivateCurrentUnitPanels(currentUnits, board);
        ActiveateRecruitableUnitPanels(recruitableUnits, board);
    }

    void ActivateCurrentUnitPanels(List<UnitData> currentUnits, Board board)
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

        SetupPlaceUnitPanels(sortedUnits, board);
    }

    void ActiveateRecruitableUnitPanels(List<RecruitableUnit> recruitableUnits, Board board)
    {
        foreach (RecruitUnitPanel panel in m_recruitUnitPanels.Values)
        {
            panel.Deactivate();
        }

        if (recruitableUnits == null || recruitableUnits.Count == 0)
        {
            return;
        }

        //Dictionary<UnitPrototype, int> sortedUnits = new Dictionary<UnitPrototype, int>();
        //foreach (UnitPrototype unit in recruitableUnits)
        //{
        //    if (!sortedUnits.ContainsKey(unit))
        //    {
        //        sortedUnits[unit] = 0;
        //    }

        //    sortedUnits[unit]++;
        //}

        SetupRecruitUnitPanels(recruitableUnits, board);
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

        foreach (RecruitUnitPanel panel in m_recruitUnitPanels.Values)
        {
            panel.Deactivate();
        }

        gameObject.SetActive(false);
    }

    void SetupPlaceUnitPanels(Dictionary<string, List<UnitData>> sortedUnits, Board board)
    {
        foreach(string type in sortedUnits.Keys)
        {
            SetupPlaceUnitPanel(type, sortedUnits[type]);
            
            //if (!m_unitPanels.ContainsKey(type))
            //{
            //    GameObject go = Instantiate(m_unitPanelPrefab, m_buttonsPanelTransform);
            //    go.transform.SetAsFirstSibling();
            //    m_unitPanels[type] = go.GetComponent<PlaceUnitPanel>();
            //}

            //m_unitPanels[type].UpdatePanel(sortedUnits[type]);
        }

    }

    void SetupPlaceUnitPanel(string type, List<UnitData> units)
    {
        if (!m_placeUnitPanels.ContainsKey(type))
        {
            GameObject go = Instantiate(m_placeUnitPanelPrefab, m_buttonsParentTransform);
            //go.transform.SetAsFirstSibling();
            go.transform.SetSiblingIndex(m_placeUnitButtonsRoot.transform.GetSiblingIndex() + 1);
            m_placeUnitPanels[type] = go.GetComponent<PlaceUnitPanel>();
        }

        m_placeUnitPanels[type].UpdatePanel(units);
    }

    public void AddUnitToPlace(UnitPrototype proto)
    {
        UnitData unit = new UnitData(proto, Vector2Int.zero, Faction.Player);
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

    void SetupRecruitUnitPanels(List<RecruitableUnit> sortedProtos, Board board)
    {
        foreach(RecruitableUnit unit in sortedProtos)
        {
            SetupRecruitUnitPanel(unit.Proto.Type, unit.Proto, unit.Count, board);
        }
    }

    void SetupRecruitUnitPanel(string type, UnitPrototype proto, int count, Board board)
    {
        if (!m_recruitUnitPanels.ContainsKey(type))
        {
            GameObject go = Instantiate(m_recruitUnitPanelPrefab, m_buttonsParentTransform);
            //go.transform.SetAsFirstSibling();
            go.transform.SetSiblingIndex(m_recruitUnitButtonsRoot.transform.GetSiblingIndex() + 1);
            m_recruitUnitPanels[type] = go.GetComponent<RecruitUnitPanel>();
        }

        m_recruitUnitPanels[type].Activate(proto, count, this, board);
    }

}
