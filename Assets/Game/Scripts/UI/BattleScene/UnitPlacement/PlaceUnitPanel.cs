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

    public List<UnitData> AvailableUnits { get; protected set; }

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
        
        AvailableUnits = units;
        m_availableUnitsText.text = AvailableUnits.Count.ToString();
        m_buttonText.text = type;
        gameObject.SetActive(true);
    }

    public void AddUnit(UnitData unit)
    {
        if(AvailableUnits == null || AvailableUnits.Count == 0)
        {
            AvailableUnits = new List<UnitData>();
        } 
        else 
        if(unit.Type != AvailableUnits[0].Type)
        {
            Debug.LogError($"Inconsistent unit types. Base type: {unit.Type}, type at index {0}: {AvailableUnits[0].Type}");
            return;
        }

        AvailableUnits.Add(unit);
        UpdatePanel(AvailableUnits);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        AvailableUnits = null;
    }

    public void SetUnitToBePlaced()
    {
        m_inputManager.SelectUnitToPlace(AvailableUnits[0], Faction.Player);
        m_inputManager.UnitPlacedEvent += OnUnitPlaced;
        m_inputManager.CancelPlacementEvent += UnsubscribeFromPlacement;
    }

    void OnUnitPlaced()
    {
        AvailableUnits.RemoveAt(0);
        UnsubscribeFromPlacement();
        if(AvailableUnits.Count == 0)
        {
            Deactivate();
            return;
        }

        UpdatePanel(AvailableUnits);
    }

    void UnsubscribeFromPlacement()
    {
        m_inputManager.UnitPlacedEvent -= OnUnitPlaced;
        m_inputManager.CancelPlacementEvent -= UnsubscribeFromPlacement;
    }
}
