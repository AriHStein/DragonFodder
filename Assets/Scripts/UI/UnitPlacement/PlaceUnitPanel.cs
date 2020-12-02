using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlaceUnitPanel : MonoBehaviour
{
    InputManager m_inputManager;

    [SerializeField] TextMeshProUGUI m_availableUnitsText = default;
    [SerializeField] TextMeshProUGUI m_buttonText = default;
    //[SerializeField] PlaceUnitButton m_button = default;

    List<UnitData> m_availableUnits;

    private void Start()
    {
        m_inputManager = FindObjectOfType<InputManager>();
    }

    public void UpdatePanel(List<UnitData> units)
    {
        if(units == null || units.Count == 0)
        {
            Debug.LogError($"Invalid units list.");
            Deactivate();
            return;
            //m_availableUnitsText.text = "";
            //m_buttonText.text = "";
        }

        string type = units[0].Type;
        for(int i = 0; i < units.Count; i++)
        {
            if(units[i].Type != type)
            {
                Debug.LogError($"Inconsistent unit types. Base type: {type}, type at index {i}: {units[i].Type}");
                Deactivate();
                return;
            }
        }
        
        m_availableUnits = units;
        m_availableUnitsText.text = m_availableUnits.Count.ToString();
        m_buttonText.text = type;
        //m_button.SetupButton(Board.Current.UnitManager.GetUnitPrototypeOfType(type), Faction.Player);
        gameObject.SetActive(true);
    }

    public void AddUnit(UnitData unit)
    {
        if(m_availableUnits == null || m_availableUnits.Count == 0)
        {
            m_availableUnits = new List<UnitData>();
        } 
        else 
        if(unit.Type != m_availableUnits[0].Type)
        {
            Debug.LogError($"Inconsistent unit types. Base type: {unit.Type}, type at index {0}: {m_availableUnits[0].Type}");
            return;
        }

        m_availableUnits.Add(unit);
        UpdatePanel(m_availableUnits);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        m_availableUnits = null;
    }

    public void SetUnitToBePlaced()
    {
        m_inputManager.SelectUnitToPlace(m_availableUnits[0], Faction.Player);
        m_inputManager.UnitPlacedEvent += OnUnitPlaced;
        m_inputManager.CancelPlacementEvent += UnsubscribeFromPlacement;
    }

    void OnUnitPlaced()
    {
        m_availableUnits.RemoveAt(0);
        UnsubscribeFromPlacement();
        if(m_availableUnits.Count == 0)
        {
            Deactivate();
            return;
        }

        UpdatePanel(m_availableUnits);
    }

    void UnsubscribeFromPlacement()
    {
        m_inputManager.UnitPlacedEvent -= OnUnitPlaced;
        m_inputManager.CancelPlacementEvent -= UnsubscribeFromPlacement;
    }
}
