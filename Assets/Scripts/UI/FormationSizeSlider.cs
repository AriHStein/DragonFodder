using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FormationSizeSlider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_formationSizeText = default;
    [SerializeField] Slider m_slider = default;

    public int FormationSize { get; protected set; }

    private void OnEnable()
    {
        OnSliderValueChanged();
    }

    public void OnSliderValueChanged()
    {
        FormationSize = (int)m_slider.value;
        m_formationSizeText.text = FormationSize.ToString();
    }
}
