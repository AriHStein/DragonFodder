using System.Collections.Generic;
using UnityEngine;

public enum PlayMode { UnitPlacement, SquadEditor, Battle }
public class Board : MonoBehaviour
{
    public static Board Current;
    public PlayMode PlayMode { get; protected set; }
    public UnitManager UnitManager { get; protected set; }


    [SerializeField] List<GameObject> m_unitPrefabs = default;

    [SerializeField] GameObject m_unitPlacementModePanel = default;
    [SerializeField] GameObject m_squadEditorModePanel = default;

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] int m_width = 8;
    [SerializeField] int m_height = 8;
    [SerializeField] int m_rowsAllowedForUnitPlacement = 3;

    [SerializeField] List<UnitData> m_preplacedUnits = default;
    [SerializeField] List<SquadData> m_preplacedSquads = default;

    [SerializeField] float m_timeBetweenTurns = 2f;
    float m_timeUntilNextTurn;

    public BoardSquare[,] Squares { get; protected set; }
    public BoardSquare GetSquareAt(int x, int y)
    {
        if (x >= Squares.GetLength(0) || y >= Squares.GetLength(1) || x < 0 || y < 0)
        {
            return null;
        }

        return Squares[x, y];
    }


    public void Awake()
    {
        UnitManager = new UnitManager(m_unitPrefabs);
        Current = this;
        //PlayMode = PlayMode.UnitPlacement;

        m_timeUntilNextTurn = m_timeBetweenTurns;

        SetupSquares();
        SetupUnits();

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = m_width / 2f;
        Camera.main.transform.position = cameraPos;
    }

    private void Start()
    {
        EnterUnitPlacementMode();
    }

    void SetupSquares()
    {
        Squares = new BoardSquare[m_width, m_height];
        
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                BoardSquare newSquare;
                if ((x + y) % 2 == 0)
                {
                    newSquare = Instantiate(m_whiteSquare, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<BoardSquare>();

                } else
                {
                    newSquare = Instantiate(m_blackSquare, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<BoardSquare>();
                }
                newSquare.Position = new Vector2Int(x, y);
                Squares[x, y] = newSquare;
            }
        }
    }

    void SetupUnits()
    {
        //foreach(UnitData unit in m_preplacedUnits)
        //{
        //    //Unit unit = Instantiate(data.Prefab,  new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
        //    //unit.Initialize(Squares[data.Position.x, data.Position.y], data);
        //    TryPlaceNewUnit(unit);
        //}

        foreach(SquadData squad in m_preplacedSquads)
        {
            TryPlaceSquad(squad);
        }
    }

    private void Update()
    {
        switch(PlayMode)
        {
            case PlayMode.Battle:
                PlayModeUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementUpdate();
                break;

            case PlayMode.SquadEditor:
                SquadEditorUpdate();
                break;

            default:
                return;
        }
    }

    void PlayModeUpdate()
    {
        if (TimeManager.Paused)
        {
            return;
        }


        if (m_timeUntilNextTurn <= 0)
        {
            UnitManager.DoUnitTurns();
            m_timeUntilNextTurn = m_timeBetweenTurns;
        }
        else
        {
            m_timeUntilNextTurn -= Time.deltaTime;
        }
    }

    void UnitPlacementUpdate()
    {

    }

    void SquadEditorUpdate()
    {

    }

    private void LateUpdate()
    {
        switch (PlayMode)
        {
            case PlayMode.Battle:
                PlayModeLateUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementLateUpdate();
                break;

            case PlayMode.SquadEditor:
                SquadEditorLateUpdate();
                break;

            default:
                return;
        }
    }

    void PlayModeLateUpdate()
    {
        UnitManager.CleanupDeadUnits();
    }

    void UnitPlacementLateUpdate()
    {

    }

    void SquadEditorLateUpdate()
    {

    }

    public void EnterBattleMode()
    {
        PlayMode = PlayMode.Battle;
        m_unitPlacementModePanel.SetActive(false);
        m_squadEditorModePanel.SetActive(false);

        ActivateSquares(Squares.GetLength(1));
    }

    public void EnterUnitPlacementMode()
    {
        PlayMode = PlayMode.UnitPlacement;
        m_unitPlacementModePanel.SetActive(true);
        m_squadEditorModePanel.SetActive(false);

        ActivateSquares(m_rowsAllowedForUnitPlacement);
    }

    public void EnterSquadEditorMode()
    {
        PlayMode = PlayMode.SquadEditor;
        m_unitPlacementModePanel.SetActive(false);
        m_squadEditorModePanel.SetActive(true);

        ActivateSquares(m_rowsAllowedForUnitPlacement);
    }

    public void SaveBoardAsSquad(bool mirrorBoard = true)
    {
        SquadData squad = GetBoardAsSquad(mirrorBoard);
        UnitSaveLoadUtility.SaveSquad(squad, "squad");
    }

    public SquadData GetBoardAsSquad(bool mirrorBoard = false)
    {
        Vector2Int origin = mirrorBoard ? new Vector2Int(Squares.GetLength(0), Squares.GetLength(1)) : Vector2Int.zero;
        List<UnitData> units = new List<UnitData>();
        foreach (Unit unit in UnitManager.Units)
        {
            UnitData newUnit = new UnitData(unit);
            if (mirrorBoard)
            {
                newUnit.Position = new Vector2Int(Squares.GetLength(0), Squares.GetLength(1)) - newUnit.Position;
            }
            units.Add(newUnit);
        }

        return SquadData.GetNewSquad(units, origin);
    }

    void ActivateSquares(int rows)
    {
        for (int x = 0; x < Squares.GetLength(0); x++)
        {
            for (int y = 0; y < Squares.GetLength(1); y++)
            {
                if (y < rows)
                {
                    Squares[x, y].Activate();
                }
                else
                {
                    Squares[x, y].Deactivate();
                }
            }
        }
    }

    public bool TryMoveUnitTo(Unit unit, BoardSquare target)
    {
        if(target == null || unit == null)
        {
            return false;
        }
        
        if (target.Unit != null)
        {
            return false;
        }

        unit.Square.Unit = null;
        unit.Square = target;
        unit.transform.position = target.transform.position;
        target.Unit = unit;

        return true;
    }

    public bool TryPlaceNewUnit(UnitData data)
    {
        if(data.Position.x < 0 || data.Position.x > Squares.GetLength(0) ||
            data.Position.y < 0 || data.Position.y > Squares.GetLength(1) ||
            Squares[data.Position.x, data.Position.y].Unit != null)
        {
            return false;
        }

        GameObject prefab = UnitManager.GetPrefabOfType(data.Type);
        if(prefab == null)
        {
            return false;
        }
        
        Unit unit = Instantiate(prefab, new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
        unit.Initialize(Squares[data.Position.x, data.Position.y], data);
        return true;
    }

    public bool TryPlaceSquad(SquadData data)
    {
        if(data == null || 
            data.Units == null || 
            data.Units.Count == 0)
        {
            return false;
        }

        data.UpdateSize();
        if(data.SquadOrigin.x < 0 || 
            data.SquadOrigin.y < 0 || 
            data.SquadOrigin.x + data.Size.x > Squares.GetLength(0) || 
            data.SquadOrigin.y + data.Size.y > Squares.GetLength(1))
        {
            return false;
        }

        bool allUnitsPlaced = true;
        foreach(UnitData unit in data.Units)
        {
            UnitData clone = unit.Clone();
            clone.Position += data.SquadOrigin;
            allUnitsPlaced = TryPlaceNewUnit(clone) && allUnitsPlaced;
        }

        return allUnitsPlaced;
    }
}
