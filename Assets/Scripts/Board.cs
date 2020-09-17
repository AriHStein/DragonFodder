using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayMode { UnitPlacement, SquadEditor, Battle, Paused }
public class Board : MonoBehaviour
{
    public static Board Current;
    public PlayMode PlayMode { get; protected set; }
    public UnitManager UnitManager { get; protected set; }

    public Action<PlayMode> PlayModeChangedEvent; 

    [SerializeField] List<GameObject> m_unitPrefabs = default;

    [SerializeField] GameObject m_unitPlacementModePanel = default;
    [SerializeField] GameObject m_squadEditorModePanel = default;
    [SerializeField] GameObject m_gameOverPanel = default;

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] int m_defaultWidth = 8;
    [SerializeField] int m_defaultHeight = 8;
    [SerializeField] int m_rowsAllowedForUnitPlacement = 3;
    [SerializeField] Vector2Int m_enemySquadSize = Vector2Int.one;

    //[SerializeField] List<UnitData> m_preplacedUnits = default;
    //[SerializeField] List<SquadData> m_preplacedSquads = default;

    [SerializeField] float m_timeBetweenTurns = 2f;
    float m_timeUntilNextTurn;

    SquadData m_playerSquadStartPosition;
    SquadData m_enemySquadStartPosition;

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

        SetupSquares(m_defaultWidth, m_defaultHeight);
        //SetupUnits();
        PositionCamera();
    }

    private void Start()
    {
        //EnterPlayMode(PlayMode.UnitPlacement);
        StartGame();
    }

    void PositionCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = Squares.GetLength(0) / 2f;
        Camera.main.transform.position = cameraPos;
    }

    public void SetupSquares(int width, int height, bool keepUnits = false)
    {
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
            TryPlaceSquad(units);
        }

        PositionCamera();
    }

    //public void ActivateSquares(int columns, int rows, bool clearBoard = false)
    //{
    //    if (columns > Squares.GetLength(0) || rows > Squares.GetLength(1))
    //    {
    //        SetupSquares(columns, rows, clearBoard);
    //    }

    //    for (int x = 0; x < Squares.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < Squares.GetLength(1); y++)
    //        {
    //            if (y < rows && x < columns)
    //            {
    //                Squares[x, y].Activate();
    //            }
    //            else
    //            {
    //                Squares[x, y].Deactivate();
    //            }
    //        }
    //    }

    //    PositionCamera();
    //}

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

    public void StartGame()
    {
        m_unitPlacementModePanel.SetActive(false);
        m_squadEditorModePanel.SetActive(false);
        m_gameOverPanel.SetActive(false);
        ClearBoard();
        PlayMode = PlayMode.UnitPlacement;

        m_unitPlacementModePanel.SetActive(true);
        SetupSquares(Squares.GetLength(0), m_rowsAllowedForUnitPlacement);
        LoadPlayerSquad();
    }

    //void SetupUnits()
    //{
    //    //foreach(UnitData unit in m_preplacedUnits)
    //    //{
    //    //    //Unit unit = Instantiate(data.Prefab,  new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
    //    //    //unit.Initialize(Squares[data.Position.x, data.Position.y], data);
    //    //    TryPlaceNewUnit(unit);
    //    //}

    //    //foreach(SquadData squad in m_preplacedSquads)
    //    //{
    //    //    TryPlaceSquad(squad);
    //    //}
    //}



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
        UnitManager.CleanupDeadUnits();
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

    public void EnterPlayMode(PlayMode mode)
    {
        if(PlayMode == mode)
        {
            return;
        }

        PlayMode = mode;

        switch(PlayMode)
        {
            case PlayMode.Battle:
                m_unitPlacementModePanel.SetActive(false);
                m_squadEditorModePanel.SetActive(false);
                m_gameOverPanel.SetActive(false);
                ClearBoard();
                SetupSquares(Squares.GetLength(0), Squares.GetLength(1));
                m_playerSquadStartPosition = LoadPlayerSquad();
                m_enemySquadStartPosition = LoadEnemySquad(true);
                break;

            case PlayMode.UnitPlacement:
                m_unitPlacementModePanel.SetActive(true);
                m_squadEditorModePanel.SetActive(false);
                m_gameOverPanel.SetActive(false);
                ClearBoard();

                SetupSquares(Squares.GetLength(0), m_rowsAllowedForUnitPlacement);
                LoadPlayerSquad();
                break;

            case PlayMode.SquadEditor:
                m_unitPlacementModePanel.SetActive(false);
                m_squadEditorModePanel.SetActive(true);
                m_gameOverPanel.SetActive(false);
                ClearBoard();
                SetupSquares(m_enemySquadSize.x, m_enemySquadSize.y);
                LoadEnemySquad(false);
                break;

            case PlayMode.Paused:
                break;

            default:
                Debug.LogError($"Enter PlayMode {mode} not implemented.");
                break;
        }

        PlayModeChangedEvent?.Invoke(PlayMode);
    }

    //void EnterBattleMode()
    //{
    //    PlayMode = PlayMode.Battle;
    //    m_unitPlacementModePanel.SetActive(false);
    //    m_squadEditorModePanel.SetActive(false);
    //    ClearBoard();
    //    ActivateSquares(Squares.GetLength(1));
    //    m_playerSquadStartPosition = LoadPlayerSquad();
    //    m_enemySquadStartPosition = LoadEnemySquad(true);
    //}

    public void ExitBattleMode(bool playerWon)
    {
        if(PlayMode != PlayMode.Battle)
        {
            Debug.Log("Exit Battle Mode called while not in BattleMode. Most likely, the battle ended with all units dead.");
            return;
        }

        if(playerWon)
        {
            BattleWon();
        } else
        {
            BattleLost();
        }
    }

    void BattleWon()
    {
        SquadData survivingUnits = m_playerSquadStartPosition.UpdateUnitStatuses(UnitManager.Units);
        UnitSaveLoadUtility.SaveSquad(survivingUnits, "Player");
        EnterPlayMode(PlayMode.UnitPlacement);
    }

    void BattleLost()
    {
        EnterPlayMode(PlayMode.Paused);
        m_gameOverPanel.SetActive(true);
    }

    //public void EnterUnitPlacementMode()
    //{
    //    PlayMode = PlayMode.UnitPlacement;
    //    m_unitPlacementModePanel.SetActive(true);
    //    m_squadEditorModePanel.SetActive(false);
    //    ClearBoard();
    //    ActivateSquares(m_rowsAllowedForUnitPlacement);

    //    LoadPlayerSquad();
    //}

    //public void EnterSquadEditorMode()
    //{
    //    PlayMode = PlayMode.SquadEditor;
    //    m_unitPlacementModePanel.SetActive(false);
    //    m_squadEditorModePanel.SetActive(true);
    //    ClearBoard();
    //    ActivateSquares(m_rowsAllowedForUnitPlacement);

    //    LoadEnemySquad(false);
    //}

    SquadData LoadPlayerSquad()
    {
        SquadData playerSquad = UnitSaveLoadUtility.LoadSquad("Player");
        TryPlaceSquad(playerSquad);

        return playerSquad;
    }

    SquadData LoadEnemySquad(bool mirror = false)
    {
        SquadData enemySquad = UnitSaveLoadUtility.LoadSquad("Enemy");
        TryPlaceSquad(enemySquad, mirror);

        return enemySquad;
    }

    public void SaveBoardAsSquad(string name, bool mirrorBoard = true)
    {
        SquadData squad = GetBoardAsSquad(mirrorBoard);
        UnitSaveLoadUtility.SaveSquad(squad, name);
    }

    public SquadData GetBoardAsSquad(bool mirrorBoard = false)
    {
        Vector2Int origin = mirrorBoard ? MirrorPosition(Vector2Int.zero) : Vector2Int.zero;
        List<UnitData> units = new List<UnitData>();
        foreach (Unit unit in UnitManager.Units)
        {
            UnitData newUnit = new UnitData(unit, origin);
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

        return true;
    }

    public bool TryPlaceNewUnit(UnitData data)
    {
        if(data.Position.x < 0 || data.Position.x >= Squares.GetLength(0) ||
            data.Position.y < 0 || data.Position.y >= Squares.GetLength(1) ||
            Squares[data.Position.x, data.Position.y].gameObject.activeInHierarchy == false || 
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

    public bool TryPlaceSquad(SquadData data, bool mirror = false)
    {
        if(data.Units == null || 
            data.Units.Count == 0)
        {
            return false;
        }

        data.UpdateSize();
        Vector2Int origin = mirror ? MirrorPosition(data.SquadOrigin) : data.SquadOrigin;
        Vector2Int size = mirror ? MirrorPosition(data.SquadOrigin) + data.Size : data.SquadOrigin + data.Size;

        if (origin.x < 0 ||
            origin.y < 0 ||
            size.x > Squares.GetLength(0) ||
            size.y > Squares.GetLength(1))
        {
            Debug.LogError($"Squad position out of bounds. Origin: {origin}, Size: {size}");
            return false;
        }

        bool allUnitsPlaced = true;
        foreach(UnitData unit in data.Units)
        {
            UnitData clone = unit.Clone();
            clone.Position += data.SquadOrigin;
            if (mirror)
            {
                clone.Position = MirrorPosition(clone.Position);
            }
            allUnitsPlaced = TryPlaceNewUnit(clone) && allUnitsPlaced;
        }

        return allUnitsPlaced;
    }

    Vector2Int MirrorPosition(Vector2Int pos)
    {
        return new Vector2Int(Squares.GetLength(0) - 1, Squares.GetLength(1) - 1) - pos;
    }
}
