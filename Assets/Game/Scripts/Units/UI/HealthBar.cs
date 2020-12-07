using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] Image m_healthImage = default;
    Unit m_unit;

    private void Start()
    {
        m_unit = GetComponent<Unit>();
        m_healthImage.fillAmount = (float)m_unit.CurrentHealth / (float)m_unit.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        m_healthImage.fillAmount = (float)m_unit.CurrentHealth / (float)m_unit.MaxHealth;
    }
}
