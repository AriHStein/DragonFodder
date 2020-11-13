using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public enum PlayMode { UnitPlacement, SquadEditor, Battle, Paused, Dungeon }
public class Board : MonoBehaviour
{
    public static Board Current;
    public PlayMode PlayMode { get; protected set; }
    public UnitManager UnitManager { get; protected set; }
    PathManager_AStar<BoardSquare> pathManager;
    BoardGraph moveGraph;

    public Action<PlayMode> PlayModeChangedEvent; 

    [SerializeField] List<UnitPrototype> m_unitPrototypes = default;

    [SerializeField] UnitPlacementMenu m_unitPlacementModePanel = default;
    [SerializeField] GameObject m_squadEditorModePanel = default;
    [SerializeField] GameObject m_gameOverPanel = default;
    [SerializeField] DungeonMap m_dungeonMap = default;
    [SerializeField] TMPro.TextMeshProUGUI m_currentGoldText = default;
    
    Encounter m_currentEncounter;
    int m_currentGold;

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] Vector2Int m_defaultBoardSize = Vector2Int.one;
    [SerializeField] Vector2Int m_defaultSquadSize = Vector2Int.one;

    [SerializeField] float m_endOfBattleDelayLength = 5f;
    const string m_playerSquadsDirectoryName = "Player";
    const string m_defaultPlayerSquadFileName = "PlayerDefault";
    const string m_currentPlayerSquadFileName = "PlayerCurrent";


    Squad m_currentPlayerSquad;
    //SquadData m_enemySquadStartPosition;

    public BoardSquare[,] Squares { get; protected set; }
    public BoardSquare GetSquareAt(int x, int y)
    {
        if (x >= Squares.GetLength(0) || y >= Squares.GetLength(1) || x < 0 || y < 0)
        {
            return null;
        }

        return Squares[x, y];
    }
    public BoardSquare GetSquareAt(Vector2Int pos)
    {
        return GetSquareAt(pos.x, pos.y);
    }

    public List<BoardSquare> GetSquaresInRange(Vector2Int center, float range)
    {
        List<BoardSquare> squares = new List<BoardSquare>();
        for (int x = -Mathf.CeilToInt(range); x <= Mathf.CeilToInt(range); x++)
        {
            for(int y = -Mathf.CeilToInt(range); y <= Mathf.CeilToInt(range); y++)
            {
                if (x * x + y * y > range * range)
                {
                    continue;
                }

                BoardSquare square = GetSquareAt(center + new Vector2Int(x, y));
                if(square == null)
                {
                    continue;
                }

                squares.Add(square);
            }
        }

        if(squares.Count == 0)
        {
            return null;
        }

        return squares;
    }

    public void Awake()
    {
        UnitManager = new UnitManager(m_unitPrototypes);
        pathManager = new PathManager_AStar<BoardSquare>();
        Current = this;

        SetupSquares(m_defaultBoardSize.x, m_defaultBoardSize.y);
        PositionCamera();
    }

    private void Start()
    {
        StartGame();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if(moveGraph != null && moveGraph.Walking != null)
    //    {
    //        foreach(BoardSquare square in moveGraph.Walking.Graph.Keys)
    //        {
    //            foreach(Path_Edge<BoardSquare> edge in moveGraph.Walking.Graph[square].edges)
    //            {
    //                Gizmos.DrawLine(square.transform.position, edge.node.data.transform.position);
    //            }
    //        }
    //    }
    //}

    void PositionCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = Squares.GetLength(0) / 2f;
        Camera.main.transform.position = cameraPos;
    }

    public bool SetupSquares(int width, int height, bool keepUnits = false)
    {
        if(width <= 0 || height <= 0)
        {
            Debug.LogError($"Invalid board size. Size: {width}, {height}");
            return false;
        }
        
        SquadData units = new SquadData();
        if(keepUnits && Squares != null) 
        {
            units = GetBoardAsSquad();
        }
        
        ClearBoard();
        Squares = new BoardSquare[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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

        if(keepUnits)
        {
            TryPlaceSquad(units, Vector2Int.zero);
        }

        PositionCamera();
        RefreshMoveGraph();
        return true;
    }

    public void SetSquaresInteractable(int columns, int rows)
    {
        for(int x = 0; x < Squares.GetLength(0); x++)
        {
            for(int y = 0; y < Squares.GetLength(1); y++)
            {
                Squares[x, y].Interactable = x < columns && y < rows;
            }
        }
    }

    void RefreshMoveGraph()
    {
        moveGraph = new BoardGraph(Squares);
    }

    public Path_AStar<BoardSquare> GetPath(BoardSquare start, bool flying, Func<BoardSquare, bool> endCondition, Func<BoardSquare, float> heuristic = null)
    {
        if(flying)
        {
            return (Path_AStar<BoardSquare>)pathManager.GetPath(moveGraph.Flying, start, endCondition, heuristic);
        }

        return (Path_AStar<BoardSquare>)pathManager.GetPath(moveGraph.Walking, start, endCondition, heuristic);
    }

    void ClearBoard()
    {
        if(Squares == null)
        {
            return;
        }
        
        for (int x = 0; x < Squares.GetLength(0); x++)
        {
            for (int y = 0; y < Squares.GetLength(1); y++)
            {
                if(Squares[x,y] == null)
                {
                    continue;
                }

                Squares[x, y].Clear();
                Destroy(Squares[x, y].gameObject);
                Squares[x, y] = null;
            }
        }
    }

    public void ClearUnits()
    {
        if (Squares == null)
        {
            return;
        }

        for (int x = 0; x < Squares.GetLength(0); x++)
        {
            for (int y = 0; y < Squares.GetLength(1); y++)
            {
                if (Squares[x, y] == null)
                {
                    continue;
                }

                Squares[x, y].Clear();
            }
        }
    }

    public void SetupEncounter(Encounter encounter)
    {
        if(encounter == null)
        {
            Debug.LogError("Encounter is null.");
            return;
        }

        m_currentEncounter = encounter;

        SetupSquares(encounter.BoardSize.x, encounter.BoardSize.y, false);
        TryPlaceSquad(encounter.Enemies, Vector2Int.zero, true);
        SetSquaresInteractable(Squares.GetLength(0), encounter.RowsAllowedForPlayerUnits);

        EnterPlayMode(PlayMode.UnitPlacement);
    }

    public void StartGame()
    {
        m_unitPlacementModePanel.Deactivate();
        m_squadEditorModePanel.SetActive(false);
        m_gameOverPanel.SetActive(false);
        ClearBoard();

        m_currentPlayerSquad = new Squad(GetDefaultPlayerSquad());
        ChangeGold(-m_currentGold);

        m_dungeonMap.SetupMap();
        EnterPlayMode(PlayMode.Dungeon);
    }

    public bool ChangeGold(int amount)
    {
        int newAmount = m_currentGold + amount;
        if(newAmount < 0)
        {
            return false;
        }

        m_currentGold = newAmount;
        UpdateGoldText();
        return true;
    }

    void UpdateGoldText()
    {
        m_currentGoldText.text = "Gold: " + m_currentGold.ToString();
    }

    #region Updates
    private void Update()
    {
        switch(PlayMode)
        {
            case PlayMode.Battle:
                BattleUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementUpdate();
                break;

            case PlayMode.SquadEditor:
                SquadEditorUpdate();
                break;

            case PlayMode.Paused:
                PausedUpdate();
                break;

            default:
                return;
        }
    }

    void BattleUpdate()
    {
        if (TimeManager.Paused)
        {
            return;
        }

        UnitManager.DoUnitTurns(Time.deltaTime, this);
        //if (m_timeUntilNextTurn <= 0)
        //{
        //    UnitManager.DoUnitTurns(m_timeBetweenTurns);
        //    m_timeUntilNextTurn = m_timeBetweenTurns;
        //}
        //else
        //{
        //    m_timeUntilNextTurn -= Time.deltaTime;
        //}
        //RefreshMoveGraph();
    }

    void UnitPlacementUpdate()
    {

    }

    void SquadEditorUpdate()
    {

    }

    void PausedUpdate()
    {

    }

    private void LateUpdate()
    {
        switch (PlayMode)
        {
            case PlayMode.Battle:
                BattleLateUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementLateUpdate();
                break;

            case PlayMode.SquadEditor:
                SquadEditorLateUpdate();
                break;

            case PlayMode.Paused:
                PausedLateUpdate();
                break;

            default:
                return;
        }
    }

    void BattleLateUpdate()
    {
        UnitManager.LateUpdate();
    }

    void UnitPlacementLateUpdate()
    {

    }

    void SquadEditorLateUpdate()
    {

    }

    void PausedLateUpdate()
    {

    }
    #endregion

    #region PlayMode Changes
    public void EnterPlayMode(PlayMode mode)
    {
        if(PlayMode == mode)
        {
            Debug.Log($"Already in {mode}.");
            return;
        }

        PlayMode = mode;

        switch(PlayMode)
        {
            case PlayMode.Battle:
                m_unitPlacementModePanel.Deactivate();
                m_squadEditorModePanel.SetActive(false);
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Deactivate();
                break;

            case PlayMode.UnitPlacement:
                if(m_currentPlayerSquad == null)
                {
                    m_currentPlayerSquad = new Squad(GetCurrentPlayerSquad());
                }

                m_unitPlacementModePanel.Activate(m_currentPlayerSquad.Data.Units);
                m_squadEditorModePanel.SetActive(false);
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Deactivate();
                break;

            case PlayMode.SquadEditor:
                m_unitPlacementModePanel.Deactivate();
                m_squadEditorModePanel.SetActive(true);
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Deactivate();
                ClearBoard();
                SetupSquares(m_defaultSquadSize.x, m_defaultSquadSize.y);
                SetSquaresInteractable(Squares.GetLength(0), Squares.GetLength(1));
                break;

            case PlayMode.Paused:
                break;

            case PlayMode.Dungeon:
                m_unitPlacementModePanel.Deactivate();
                m_squadEditorModePanel.SetActive(false);
                m_gameOverPanel.SetActive(false);
                m_dungeonMap.Activate();
                break;

            default:
                Debug.LogError($"Enter PlayMode {mode} not implemented.");
                break;
        }

        PlayModeChangedEvent?.Invoke(PlayMode);
    }

    public void ExitBattleMode(bool playerWon)
    {
        if(PlayMode != PlayMode.Battle)
        {
            Debug.Log("Exit Battle Mode called while not in BattleMode. Most likely, the battle ended with all units dead.");
            return;
        }

        UnitManager.TriggerVictoryAnimations();
        EnterPlayMode(PlayMode.Paused);
        if(playerWon)
        {
            Invoke(nameof(BattleWon), m_endOfBattleDelayLength);
        } else
        {
            Invoke(nameof(BattleLost), m_endOfBattleDelayLength);
        }
    }

    void BattleWon()
    {
        m_currentPlayerSquad.UpdateUnits(UnitManager.Units);
        UnitSaveLoadUtility.SaveSquad(m_currentPlayerSquad.Data, m_currentPlayerSquadFileName, m_playerSquadsDirectoryName);

        ChangeGold(m_currentEncounter.Reward);

        m_dungeonMap.ExitEncounter(m_currentEncounter, true);
        EnterPlayMode(PlayMode.Dungeon);
    }

    void BattleLost()
    {
        m_dungeonMap.ExitEncounter(m_currentEncounter, false);
        
        EnterPlayMode(PlayMode.Paused);
        m_gameOverPanel.SetActive(true);
    }
    #endregion

    #region Save/Load
    SquadData GetCurrentPlayerSquad()
    {
        return UnitSaveLoadUtility.LoadSquad(m_currentPlayerSquadFileName, m_playerSquadsDirectoryName);
    }

    SquadData GetDefaultPlayerSquad()
    {
        return UnitSaveLoadUtility.LoadSquad(m_defaultPlayerSquadFileName, m_playerSquadsDirectoryName);
    }

    public SquadData LoadAndPlaceEnemyFormation(int size, bool mirror = false)
    {
        SquadData formation = SquadBuilder.GenerateFormationFromEnemySquads(size);

        Vector2Int boardSize = new Vector2Int(
            Mathf.Max(formation.Size.x + 1, m_defaultBoardSize.x),
            Mathf.Max(formation.Size.y + 1, m_defaultBoardSize.y));

        SetupSquares(boardSize.x, boardSize.y);
        TryPlaceSquad(formation, Vector2Int.zero, mirror);

        return formation;
    }

    public SquadData LoadSquadToBoard(string name, string directory, Vector2Int offset, bool resizeBoard = false, bool mirror = false)
    {
        SquadData squad = UnitSaveLoadUtility.LoadSquad(name, directory);
        if(resizeBoard)
        {
            SetupSquares(squad.Size.x + 1, squad.Size.y + 1);
        }
        
        TryPlaceSquad(squad, offset, mirror);

        return squad;
    }

    public void SaveBoardAsSquad(string name, string dir, bool mirrorBoard = true)
    {
        SquadData squad = GetBoardAsSquad(mirrorBoard);
        UnitSaveLoadUtility.SaveSquad(squad, name, dir);
    }
    #endregion

    #region Unit Movement and Placing
    public SquadData GetBoardAsSquad(bool mirrorBoard = false)
    {
        Vector2Int origin = mirrorBoard ? MirrorPosition(Vector2Int.zero) : Vector2Int.zero;
        List<UnitSerializationData> units = new List<UnitSerializationData>();
        foreach (Unit unit in UnitManager.Units)
        {
            UnitSerializationData newUnit = new UnitSerializationData(unit, origin);
            //if (mirrorBoard)
            //{
            //    newUnit.Position = new Vector2Int(Squares.GetLength(0), Squares.GetLength(1)) - newUnit.Position;
            //}
            units.Add(newUnit);
        }

        return new SquadData(units, origin);
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
        RefreshMoveGraph();
        return true;
    }

    public Unit TryPlaceUnit(UnitSerializationData data)
    {
        if(data.Position.x < 0 || data.Position.x >= Squares.GetLength(0) ||
            data.Position.y < 0 || data.Position.y >= Squares.GetLength(1) ||
            Squares[data.Position.x, data.Position.y].gameObject.activeInHierarchy == false || 
            Squares[data.Position.x, data.Position.y].Unit != null)
        {
            Debug.LogWarning("Failed to place unit");
            return null;
        }

        UnitPrototype proto = UnitManager.GetUnitPrototypeOfType(data.Type);
        //GameObject prefab = UnitManager.GetPrefabOfType(data.Type);
        GameObject prefab = proto.Prefab;
        if(prefab == null)
        {
            Debug.LogWarning("Failed to place unit");
            return null;
        }
        
        Unit unit = Instantiate(prefab, new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
        unit.Initialize(this, Squares[data.Position.x, data.Position.y], data, proto);
        return unit;
    }

    public bool TryPlaceSquad(SquadData data, Vector2Int offset, bool mirror = false)
    {
        if(data.Units == null || 
            data.Units.Count == 0)
        {
            Debug.Log("No units");
            return false;
        }

        data.RecalculateParameters();
        Vector2Int origin = mirror ? MirrorPosition(data.SquadOrigin + offset) : data.SquadOrigin + offset;
        Vector2Int size = mirror ? MirrorPosition(data.SquadOrigin + offset + data.Size) : data.SquadOrigin + offset + data.Size;

        if (origin.x < 0 ||
            origin.y < 0 ||
            size.x > Squares.GetLength(0) ||
            size.y > Squares.GetLength(1))
        {
            Debug.LogError($"Squad position out of bounds. Origin: {origin}, Size: {size}");
            return false;
        }

        int failedUnitCount = 0;
        foreach(UnitSerializationData unit in data.Units)
        {
            UnitSerializationData clone = unit.Clone();
            clone.Position += data.SquadOrigin + offset;
            if (mirror)
            {
                clone.Position = MirrorPosition(clone.Position);
            }
            Unit placedUnit = TryPlaceUnit(clone);
            if(placedUnit == null)
            {
                Debug.LogWarning($"Failed to place unit {clone.Type}");
                failedUnitCount++;
                continue;
            }

            int faceDir = mirror ? -1 : 1;
            placedUnit.FaceToward(GetSquareAt(placedUnit.Square.Position.x, placedUnit.Square.Position.y + faceDir));
        }

        //Debug.LogWarning($"Failed to place {failedUnitCount} units");
        //m_currentPlayerSquad.UpdateStatuses(UnitManager.Units);
        return failedUnitCount == 0;
    }

    public Vector2Int MirrorPosition(Vector2Int pos)
    {
        return new Vector2Int(Squares.GetLength(0) - 1, Squares.GetLength(1) - 1) - pos;
    }
    #endregion
}
