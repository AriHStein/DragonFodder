using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceUnitButton : MonoBehaviour
{

    InputManager m_inputManager;

    public event System.Action<PlaceUnitButton> ButtonDeactivatedEvent;

    [SerializeField] TextMeshProUGUI m_nameText = default;
    [SerializeField] TextMeshProUGUI m_levelText = default;
    [SerializeField] Image m_unitPortrait = default;
    [SerializeField] Image m_healthBarFill = default;
    [SerializeField] TextMeshProUGUI m_healthText = default;


    private void Start()
    {
        m_inputManager = FindObjectOfType<InputManager>();
    }

    public UnitData Unit { get; protected set; }

    public void SetupButton(UnitData unit)
    {
        Unit = unit;


        m_nameText.text = Unit.Type;
        m_levelText.text = $"Level {Unit.Level}";
        UnitPrototype proto = UnitPrototypeDB.GetProto(Unit.Type);

        if(proto.Portrait != null)
        {
            m_unitPortrait.sprite = proto.Portrait;
        }

        m_healthBarFill.fillAmount = (float)Unit.CurrentHealth / (float)proto.MaxHealth;
        m_healthText.text = $"{Unit.CurrentHealth}/{proto.MaxHealth}"; 
    }

    public void SetUnitToBePlaced()
    {
        m_inputManager.SelectUnitToPlace(Unit);
        m_inputManager.UnitPlacedEvent += OnUnitPlaced;
        m_inputManager.CancelPlacementEvent += UnsubscribeFromPlacement;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        Unit = null;
        ButtonDeactivatedEvent?.Invoke(this);
    }

    void OnUnitPlaced()
    {
        Unit = null;
        UnsubscribeFromPlacement();
        Deactivate();
    }

    void UnsubscribeFromPlacement()
    {
        m_inputManager.UnitPlacedEvent -= OnUnitPlaced;
        m_inputManager.CancelPlacementEvent -= UnsubscribeFromPlacement;
    }
}
