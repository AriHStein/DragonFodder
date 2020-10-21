using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;


[RequireComponent(typeof(RectTransform))]
public class DungeonMap : MonoBehaviour
{
    private class EncounterData
    {
        public Encounter Encounter;
        //public Vector2Int Position;
        public EncounterButton Button;
        public List<EncounterData> Connections;

        public EncounterData(Encounter encounter,
            //Vector2Int position, 
            EncounterButton button,
            List<EncounterData> connections)
        {
            Encounter = encounter;
            //Position = position;
            Button = button;
            Connections = connections;
        }
    }

    [SerializeField] List<int> m_encounterDifficulties = default;
    [SerializeField] GameObject m_dugeonModePanel = default;
    [SerializeField] GameObject m_encounterButtonPrefab = default;
    [SerializeField] UILineRendererList m_connectionList = default;

    [SerializeField] Vector2Int m_encounterGridSize = default;
    [SerializeField] int m_encounterCount = 10;

    [SerializeField] Vector2 m_edgeBufferSize = 50 * Vector2.one;
    RectTransform m_rectTranform;
    EncounterData[,] m_encounterDatas;
    Dictionary<Encounter, EncounterData> m_encounters;

    //List<Encounter> m_encounters;

    private void Start()
    {
        m_rectTranform = GetComponent<RectTransform>();
        //SetupMap();
        //GenerateEncounters();
        //SetupEncounterButtons();
    }

    public void SetupMap()
    {
        if (m_rectTranform == null)
        {
            m_rectTranform = GetComponent<RectTransform>();
        }

        GenerateEncounters();
        SetupEncounterButtons();
        SetupConnections();
    }

    struct Path
    {
        public Vector2Int Start;
        public Vector2Int End;
        public Path(Vector2Int start, Vector2Int end)
        {
            Start = start;
            End = end;
        }
    }

    void GenerateEncounters()
    {
        List<Vector2Int> weightedDirections = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.up,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.left,
            Vector2Int.down
        };

        List<Vector2Int> encounterPositions = new List<Vector2Int> { new Vector2Int(m_encounterGridSize.x / 2, 0) };
        HashSet<Path> encouterPaths = new HashSet<Path>();

        int failedCount = 0;
        while (encounterPositions.Count < m_encounterCount && failedCount < 100)
        {
            Vector2Int start = encounterPositions[Random.Range(0, encounterPositions.Count)];
            Vector2Int end = start + weightedDirections[Random.Range(0, weightedDirections.Count)];
            if (end.x < 0 || end.y < 0 ||
                end.x >= m_encounterGridSize.x || end.y >= m_encounterGridSize.y)
            {
                failedCount++;
                continue;
            }

            encouterPaths.Add(new Path(start, end));
            if(!encounterPositions.Contains(end))
            {
                encounterPositions.Add(end);
            }
        }

        m_encounterDatas = new EncounterData[m_encounterGridSize.x, m_encounterGridSize.y];
        m_encounters = new Dictionary<Encounter, EncounterData>();
        //m_encounters = new List<Encounter>();
        int encounterDifficulty = 2;
        foreach(Vector2Int position in encounterPositions)
        {
            Encounter e = EncounterBuilder.GenerateEncounter(position, encounterDifficulty);
            encounterDifficulty++;

            //m_encounterDatas[position.x, position.y] = new EncounterData(e, null, new List<EncounterData>());
            m_encounters.Add(e, m_encounterDatas[position.x, position.y]);
        }

        foreach(Path path in encouterPaths)
        {
            m_encounterDatas[path.Start.x, path.Start.y].Connections.Add(m_encounterDatas[path.End.x, path.End.y]);
        }
    }

    void SetupEncounterButtons()
    {
        ClearEncounterButtons();

        float totalHeight = m_rectTranform.rect.yMax - m_rectTranform.rect.yMin - 2 * m_edgeBufferSize.y;
        float totalWidth = m_rectTranform.rect.xMax - m_rectTranform.rect.xMin - 2 * m_edgeBufferSize.x;

        Vector2Int lowerLeft = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int upperRight = Vector2Int.zero;
        for(int x = 0; x < m_encounterDatas.GetLength(0); x++)
        {
            for(int y = 0; y < m_encounterDatas.GetLength(1); y++)
            {
                EncounterData data = m_encounterDatas[x, y];
                if(data != null)
                {
                    lowerLeft.x = Mathf.Min(lowerLeft.x, x);
                    lowerLeft.y = Mathf.Min(lowerLeft.y, y);
                    upperRight.x = Mathf.Max(upperRight.x, x);
                    upperRight.y = Mathf.Max(upperRight.y, y);
                }
            }
        }

        int gridWidth = upperRight.x - lowerLeft.x;
        int gridHight = upperRight.y - lowerLeft.y;

        float gridSizeX = totalWidth / gridWidth;
        float gridSizeY = totalHeight / gridHight;

        for (int x = 0; x < m_encounterDatas.GetLength(0); x++)
        {
            for (int y = 0; y < m_encounterDatas.GetLength(1); y++)
            {
                EncounterData data = m_encounterDatas[x, y];
                if (data != null)
                {
                    data.Encounter.MapPosition -= lowerLeft;
                    
                    Vector3 buttonPos = new Vector3(data.Encounter.MapPosition.x * gridSizeX, data.Encounter.MapPosition.y * gridSizeY, 0);
                    buttonPos += (Vector3)m_edgeBufferSize;
                    SetupEncounterButton(data, buttonPos);
                }
            }
        }
    }

    void SetupEncounterButton(EncounterData data, Vector3 position)
    {
        GameObject go = Instantiate(m_encounterButtonPrefab, transform);
        EncounterButton eb = go.GetComponent<EncounterButton>();
        eb.Initialize(data.Encounter);
        RectTransform rect = eb.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        data.Button = eb;
    }

    void SetupConnections()
    {

        List<Vector2> points = new List<Vector2>();
        foreach(EncounterData data in m_encounters.Values)
        {
            if(data.Connections == null)
            {
                Debug.LogWarning("Connections is null!");
                continue;
            }

            for(int i = 0; i < data.Connections.Count; i++)
            {
                points.Add(data.Button.GetComponent<RectTransform>().anchoredPosition);
                points.Add(data.Connections[i].Button.GetComponent<RectTransform>().anchoredPosition);
                //Vector2[] points = new Vector2[2];
                //points[0] = data.Position;
                //points[1] = data.Connections[i];
                //UILineRenderer lr = Instantiate(m_connectionPrefab, transform).GetComponent<UILineRenderer>();
                //lr.Points = points;
            }
        }

        m_connectionList.Points = points;
    }

    void ClearEncounterButtons()
    {
        for (int x = 0; x < m_encounterDatas.GetLength(0); x++)
        {
            for (int y = 0; y < m_encounterDatas.GetLength(1); y++)
            {
                EncounterData data = m_encounterDatas[x, y];
                if (data != null && data.Button != null)
                {
                    Destroy(data.Button.gameObject);
                    data.Button = null;
                }
            }
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
        if(encounter == null)
        {
            Debug.LogError("Encounter is null.");
            return;
        }

        if (m_encounters == null)
        {
            Debug.LogError($"EncounterMap is null. ?????");
            return;
        }
        
        if (!m_encounters.ContainsKey(encounter))
        {
            Debug.LogError($"Encounter not found. ?????");
            return;
        }

        encounter.Complete = encounterWon;
        if (encounterWon)
        {
            m_encounters[encounter].Button.DisableButton();
        }
    }
}
