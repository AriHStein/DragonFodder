using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class ManaBar : MonoBehaviour
{
    [SerializeField] Image m_manaImage = default;
    Unit m_unit;

    private void Start()
    {
        m_unit = GetComponent<Unit>();
        m_manaImage.fillAmount = (float)m_unit.CurrentMP / (float)m_unit.MaxMP;
    }

    // Update is called once per frame
    void Update()
    {
        m_manaImage.fillAmount = (float)m_unit.CurrentMP / (float)m_unit.MaxMP;
    }
}
