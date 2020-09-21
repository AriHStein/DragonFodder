using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : MonoBehaviour
{
    [SerializeField] int m_encounterCount = 4;
    [SerializeField] GameObject m_dugeonModePanel = default;
    [SerializeField] GameObject m_encounterButtonPrefab = default;

    [SerializeField] float m_topBottomBuffer = 2f;
    RectTransform rectTranform;
    List<Encounter> m_encounters;
    List<EncounterButton> m_encouterButtons;

    private void Start()
    {
        rectTranform = GetComponent<RectTransform>();
        
        GenerateEncounters();
        SetupEncounterButtons();
    }

    void GenerateEncounters()
    {
        m_encounters = new List<Encounter>();
        for(int i = 0; i < m_encounterCount; i++)
        {
            Encounter e = EncounterBuilder.GenerateEncounter(i + 1);
            m_encounters.Add(e);
        }
    }

    void SetupEncounterButtons()
    {
        ClearEncounterButtons();

        m_encouterButtons = new List<EncounterButton>();
        float totalHeight = rectTranform.rect.yMax - rectTranform.rect.yMin - 2*m_topBottomBuffer;
        Vector3 startPos = new Vector3(0, rectTranform.rect.yMin + m_topBottomBuffer, 0);
        Vector3 offest = Vector3.up * totalHeight / (m_encounters.Count - 1);
        for(int i = 0; i < m_encounters.Count; i++)
        {
            SetupEncounterButton(m_encounters[i], startPos + offest * i);
        }
    }

    void SetupEncounterButton(Encounter encounter, Vector3 position)
    {
        GameObject go = Instantiate(m_encounterButtonPrefab, transform);
        EncounterButton eb = go.GetComponent<EncounterButton>();
        eb.Initialize(encounter, m_dugeonModePanel);
        RectTransform rect = eb.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }

    void ClearEncounterButtons()
    {
        if (m_encouterButtons != null)
        {
            for (int i = m_encouterButtons.Count - 1; i >= 0; i--)
            {
                Destroy(m_encouterButtons[i].gameObject);
            }
        }
    }
}
