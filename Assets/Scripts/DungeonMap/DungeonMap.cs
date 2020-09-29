using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DungeonMap : MonoBehaviour
{
    [SerializeField] int m_encounterCount = 4;
    [SerializeField] GameObject m_dugeonModePanel = default;
    [SerializeField] GameObject m_encounterButtonPrefab = default;

    [SerializeField] float m_topBottomBuffer = 2f;
    RectTransform m_rectTranform;
    List<Encounter> m_encounters;
    Dictionary<Encounter, EncounterButton> m_encouterButtons;

    private void Start()
    {
        m_rectTranform = GetComponent<RectTransform>();
        //SetupMap();
        //GenerateEncounters();
        //SetupEncounterButtons();
    }

    public void SetupMap()
    {
        if(m_rectTranform == null)
        {
            m_rectTranform = GetComponent<RectTransform>();
        }
        
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

        m_encouterButtons = new Dictionary<Encounter, EncounterButton>();
        float totalHeight = m_rectTranform.rect.yMax - m_rectTranform.rect.yMin - 2*m_topBottomBuffer;
        Vector3 startPos = new Vector3(0, m_rectTranform.rect.yMin + m_topBottomBuffer, 0);
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
        eb.Initialize(encounter);
        RectTransform rect = eb.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        m_encouterButtons[encounter] = eb;
    }

    void ClearEncounterButtons()
    {
        if (m_encouterButtons != null)
        {
            foreach(Encounter encounter in m_encouterButtons.Keys)
            {
                Destroy(m_encouterButtons[encounter].gameObject);
            }
            //for (int i = m_encouterButtons.Count - 1; i >= 0; i--)
            //{
            //    Destroy(m_encouterButtons[i].gameObject);
            //}

            m_encouterButtons = null;
        }
    }

    public void Activate()
    {
        m_dugeonModePanel.SetActive(true);
    }

    public void Deactivate()
    {
        m_dugeonModePanel.SetActive(false);
    }

    public void ExitEncounter(Encounter encounter, bool encounterWon)
    {
        if(m_encouterButtons == null)
        {
            Debug.LogError("Encounter buttons is null. ??????");
            return;
        }

        if(encounter == null)
        {
            Debug.LogError("Encounter is null.");
            return;
        }

        if(!m_encouterButtons.ContainsKey(encounter))
        {
            Debug.LogError($"Encounter not found. ?????");
            return;
        }


        encounter.Complete = encounterWon;
        if (encounterWon)
        {
            m_encouterButtons[encounter].DisableButton();
        }
    }
}
