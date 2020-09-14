﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUnitButton : MonoBehaviour
{

    InputManager m_inputManager;

    private void Start()
    {
        m_inputManager = FindObjectOfType<InputManager>();
    }

    [SerializeField] UnitPrototype m_unit = default;
    [SerializeField] Faction m_faction = default;
    public void SetUnit()
    {
        m_inputManager.SelectUnitToPlace(m_unit, m_faction);
    }
}
