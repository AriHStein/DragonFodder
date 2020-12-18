using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacementMenu : MonoBehaviour
{
    //[SerializeField] GameObject m_placeUnitPanelPrefab = default;
    [SerializeField] GameObject m_placeUnitButtonPrefab = default;
    [SerializeField] Transform m_buttonsParentTransform = default;
    [SerializeField] Transform m_placeUnitButtonsRoot = default;

    //Dictionary<string, PlaceUnitPanel> m_placeUnitPanels;
    List<PlaceUnitButton> m_buttons;
    
    // Start is called before the first frame update
    void Awake()
    {
        //m_placeUnitPanels = new Dictionary<string, PlaceUnitPanel>();
        m_buttons = new List<PlaceUnitButton>();
    }

    public void Activate(
        List<UnitData> currentUnits)
    {
        gameObject.SetActive(true);
        ActivateCurrentUnitPanels(currentUnits);
    }

    void ActivateCurrentUnitPanels(List<UnitData> currentUnits)
    {
        //foreach (PlaceUnitPanel panel in m_placeUnitPanels.Values)
        //{
        //    panel.Deactivate();
        //}
        //foreach(PlaceUnitButton button in m_buttons)
        //{
        //    button.Deactivate();
        //    Destroy(button.gameObject);
        //}

        //m_buttons = new List<PlaceUnitButton>();

        ClearButtons();
        if (currentUnits == null || currentUnits.Count == 0)
        {
            return;
        }

        foreach(UnitData unit in currentUnits)
        {
            SetupButton(unit);
        }

        //Dictionary<string, List<UnitData>> sortedUnits = new Dictionary<string, List<UnitData>>();
        //foreach (UnitData unit in currentUnits)
        //{
        //    if (!sortedUnits.ContainsKey(unit.Type))
        //    {
        //        sortedUnits[unit.Type] = new List<UnitData>();
        //    }

        //    sortedUnits[unit.Type].Add(unit);
        //}

        //SetupPlaceUnitPanels(sortedUnits);

    }

    public void Deactivate()
    {
        if(!gameObject.activeSelf)
        {
            return;
        }
        
        //foreach(PlaceUnitPanel panel in m_placeUnitPanels.Values)
        //{
        //    panel.Deactivate();
        //}

        foreach(PlaceUnitButton button in m_buttons)
        {
            button.Deactivate();
            Destroy(button.gameObject);
        }

        m_buttons = new List<PlaceUnitButton>();

        gameObject.SetActive(false);
    }

    void ClearButtons()
    {
        if(m_buttons == null)
        {
            m_buttons = new List<PlaceUnitButton>();
            return;
        }
        
        for(int i = m_buttons.Count; i > 0; --i)
        {
            m_buttons[i].Deactivate();
        }

        m_buttons.Clear();
        //foreach (PlaceUnitButton button in m_buttons)
        //{
        //    button.Deactivate();
        //    Destroy(button.gameObject);
        //}

        //m_buttons = new List<PlaceUnitButton>();
    }

    void OnButtonDeactivated(PlaceUnitButton button)
    {
        button.ButtonDeactivatedEvent -= OnButtonDeactivated;
        if(!m_buttons.Contains(button))
        {
            return;
        }

        m_buttons.Remove(button);
        Destroy(button.gameObject);
    }

    void SetupUnitButtons(List<UnitData> units)
    {
        foreach(UnitData unit in units)
        {
            SetupButton(unit);
        }
    }

    void SetupButton(UnitData unit)
    {
        GameObject go = Instantiate(m_placeUnitButtonPrefab, m_buttonsParentTransform);
        go.transform.SetSiblingIndex(m_placeUnitButtonsRoot.transform.GetSiblingIndex() + 1);
        PlaceUnitButton button = go.GetComponent<PlaceUnitButton>();
        button.SetupButton(unit);
        button.ButtonDeactivatedEvent += OnButtonDeactivated;
        m_buttons.Add(button);
    }

    //void SetupPlaceUnitPanels(
    //    Dictionary<string, List<UnitData>> sortedUnits)
    //{
    //    foreach(string type in sortedUnits.Keys)
    //    {
    //        SetupPlaceUnitPanel(type, sortedUnits[type]);
    //    }

    //}

    //void SetupPlaceUnitPanel(string type, List<UnitData> units)
    //{
    //    if (!m_placeUnitPanels.ContainsKey(type))
    //    {
    //        GameObject go = Instantiate(m_placeUnitPanelPrefab, m_buttonsParentTransform);
    //        go.transform.SetSiblingIndex(m_placeUnitButtonsRoot.transform.GetSiblingIndex() + 1);
    //        m_placeUnitPanels[type] = go.GetComponent<PlaceUnitPanel>();
    //    }

    //    m_placeUnitPanels[type].UpdatePanel(units);
    //}

    public List<UnitData> GetUnplacedUnits()
    {
        List<UnitData> units = new List<UnitData>();
        //foreach(PlaceUnitPanel panel in m_placeUnitPanels.Values)
        //{
        //    if(panel.AvailableUnits != null && panel.AvailableUnits.Count > 0)
        //    {
        //        units.AddRange(panel.AvailableUnits);
        //    }
        //}
        foreach(PlaceUnitButton button in m_buttons)
        {
            if(button.Unit != null)
            {
                units.Add(button.Unit);
                //button.Deactivate();
                //Destroy(button.gameObject);
            }
        }

        return units;
    }

    //public void AddUnitToPlace(UnitPrototype proto)
    //{
    //    UnitData unit = new UnitData(proto, Faction.Player);
    //    if (!m_placeUnitPanels.ContainsKey(proto.Type))
    //    {
    //        List<UnitData> units = new List<UnitData> { unit };
    //        SetupPlaceUnitPanel(proto.Type, units);
    //    }
    //    else
    //    {
    //        m_placeUnitPanels[proto.Type].AddUnit(unit);
    //    }
    //}
}
