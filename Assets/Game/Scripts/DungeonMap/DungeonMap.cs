using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;


[RequireComponent(typeof(RectTransform))]
public class DungeonMap : MonoBehaviour
{
    [SerializeField] GameState m_gameState = default;
    
    [SerializeField] GameObject m_dugeonModePanel = default;
    [SerializeField] GameObject m_encounterButtonPrefab = default;
    [SerializeField] UILineRendererList m_connectionList = default;

    [SerializeField] Vector2Int m_encounterGridSize = default;
    [SerializeField] int m_encounterCount = 10;

    [SerializeField] Vector2 m_edgeBufferSize = 50 * Vector2.one;

    RectTransform m_rectTranform;
    Dictionary<Encounter, EncounterButton> m_encounterButtons;
    Dictionary<System.Guid, Encounter> m_encounters;
    List<Encounter> m_availableEncounters;

    private void Start()
    {
        m_rectTranform = GetComponent<RectTransform>();
    }

    public void SetupMap(Board board)
    {
        if (m_rectTranform == null)
        {
            m_rectTranform = GetComponent<RectTransform>();
        }

        if(m_encounters == null || m_encounters.Keys.Count == 0)
        {
            if (m_gameState.Data.Encounters == null || m_gameState.Data.Encounters.Count == 0)
            {
                DungeonBuilder.GenerateEncounters(m_encounterCount, m_encounterGridSize, out m_encounters, out m_availableEncounters);
                m_gameState.Data.Encounters = new List<Encounter>(m_encounters.Values);
            }
            else
            {
                LoadEncounters();
            }
        }

        SetupEncounterButtons(board);
        RefreshPanel();
    }
    
    public void Activate()
    {
        m_dugeonModePanel.SetActive(true);
        RefreshPanel();
    }
    
    public void Deactivate()
    {
        m_dugeonModePanel.SetActive(false);
    }
    
    public void ExitEncounter(Encounter encounter, bool encounterWon)
    {
        if (encounter == null)
        {
            Debug.LogError("Encounter is null.");
            return;
        }

        if (m_encounterButtons == null)
        {
            Debug.LogError($"EncounterMap is null. ?????");
            return;
        }

        if (!m_encounterButtons.ContainsKey(encounter))
        {
            Debug.LogError($"Encounter not found. ?????");
            return;
        }

        encounter.Complete = encounterWon;
        if (encounterWon)
        {
            m_encounterButtons[encounter].DisableButton();
            foreach (Encounter connection in encounter.Connections)
            {
                m_availableEncounters.Add(connection);
                connection.Available = true;
            }

            m_gameState.SaveGame();
        }
        else
        {
            m_gameState.GameOver();
        }


        RefreshPanel();
    }

    void RefreshPanel()
    {
        SetupConnections();
        ActivateButtons();
    }

    void LoadEncounters()
    {
        m_encounters = new Dictionary<System.Guid, Encounter>();
        m_availableEncounters = new List<Encounter>();

        foreach(Encounter e in m_gameState.Data.Encounters)
        {
            m_encounters[e.ID] = e;
            if(e.Available)
            {
                m_availableEncounters.Add(e);
            }
        }

        foreach(Encounter e in m_encounters.Values)
        {
            e.FormAllConnections(m_encounters);
        }
    }
    
    void SetupEncounterButtons(Board board)
    {
        ClearEncounterButtons();
        m_encounterButtons = new Dictionary<Encounter, EncounterButton>();
        float totalHeight = m_rectTranform.rect.yMax - m_rectTranform.rect.yMin - 2 * m_edgeBufferSize.y;
        float totalWidth = m_rectTranform.rect.xMax - m_rectTranform.rect.xMin - 2 * m_edgeBufferSize.x;

        Vector2Int lowerLeft = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int upperRight = Vector2Int.zero;
        foreach(Encounter encounter in m_encounters.Values)
        {
            lowerLeft.x = Mathf.Min(lowerLeft.x, encounter.MapPosition.x);
            lowerLeft.y = Mathf.Min(lowerLeft.y, encounter.MapPosition.y);
            upperRight.x = Mathf.Max(upperRight.x, encounter.MapPosition.x);
            upperRight.y = Mathf.Max(upperRight.y, encounter.MapPosition.y);
        }

        int gridWidth = upperRight.x - lowerLeft.x;
        int gridHight = upperRight.y - lowerLeft.y;

        float gridSizeX = totalWidth / gridWidth;
        float gridSizeY = totalHeight / gridHight;

        foreach (Encounter encounter in m_encounters.Values)
        {
            encounter.MapPosition -= lowerLeft;

            Vector3 buttonPos = new Vector3(encounter.MapPosition.x * gridSizeX, encounter.MapPosition.y * gridSizeY, 0);
            buttonPos += (Vector3)m_edgeBufferSize;
            SetupEncounterButton(encounter, buttonPos, board);
        }
    }
    
    void SetupEncounterButton(Encounter encounter, Vector3 position, Board board)
    {
        GameObject go = Instantiate(m_encounterButtonPrefab, transform);
        EncounterButton eb = go.GetComponent<EncounterButton>();
        eb.Initialize(encounter, board);
        RectTransform rect = eb.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        m_encounterButtons[encounter] = eb;
    }
    
    void ClearEncounterButtons()
    {
        if(m_encounterButtons == null)
        {
            return;
        }
        
        foreach(EncounterButton button in m_encounterButtons.Values)
        {
            if(button == null)
            {
                continue;
            }
            
            Destroy(button.gameObject);
        }
    }
    
    void SetupConnections()
    {
        List<Vector2> points = new List<Vector2>();
        foreach(Encounter encounter in m_availableEncounters)
        {
            // only draw connecting lines going from complete encounters to available or complete encounters.
            if(encounter.Complete)
            {
                for (int i = 0; i < encounter.Connections.Count; i++)
                {
                    // Add start + end point for each connection. Line renderer will render it as line segmenets.
                    points.Add(m_encounterButtons[encounter].GetComponent<RectTransform>().anchoredPosition);
                    points.Add(m_encounterButtons[encounter.Connections[i]].GetComponent<RectTransform>().anchoredPosition);
                }
            }
        }

        // Set points in the line renderer to draw lines for all connections between available or complete encounters
        if(points.Count == 0)
        {
            m_connectionList.gameObject.SetActive(false);
        }
        else
        {
            m_connectionList.gameObject.SetActive(true);
            m_connectionList.Points = points;
        }

    }
    
    void ActivateButtons()
    {
        foreach (Encounter encounter in m_encounters.Values)
        {
            if (m_availableEncounters.Contains(encounter))
            {
                m_encounterButtons[encounter].gameObject.SetActive(true);
                if (!encounter.Complete)
                {
                    m_encounterButtons[encounter].EnableButton();
                }
                else
                {
                    m_encounterButtons[encounter].DisableButton();
                }
            }
            else
            {
                m_encounterButtons[encounter].gameObject.SetActive(false);
            }
        }
    }
}
