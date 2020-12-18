using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public enum PlayMode { 
    UnitPlacement, 
    //SquadEditor, 
    Battle, 
    Paused, 
    Dungeon 
}
public abstract class Board_Base : MonoBehaviour
{
    public PlayMode PlayMode { get; protected set; }
    public Action<PlayMode> PlayModeChangedEvent;
    public UnitManager UnitManager { get; protected set; }
    PathManager_AStar<BoardSquare> pathManager;

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] protected Vector2Int m_defaultBoardSize = Vector2Int.one;

    protected virtual void Awake()
    {
        UnitManager = new UnitManager();
        UnitManager.BattleCompleteEvent += OnBattleComplete;
        pathManager = new PathManager_AStar<BoardSquare>();
        //Current = this;

        SetupSquares(m_defaultBoardSize.x, m_defaultBoardSize.y);
        PositionCamera();
    }

    protected virtual void Start()
    {
        StartGame();
    }

    protected abstract void StartGame();

    #region Updates
    protected void Update()
    {
        switch (PlayMode)
        {
            case PlayMode.Battle:
                BattleUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementUpdate();
                break;

            //case PlayMode.SquadEditor:
            //    SquadEditorUpdate();
            //    break;

            case PlayMode.Paused:
                PausedUpdate();
                break;

            default:
                return;
        }
    }

    protected virtual void BattleUpdate()
    {
        if (TimeManager.Paused)
        {
            return;
        }

        UnitManager.DoUnitTurns(Time.deltaTime, this);
    }

    protected virtual void UnitPlacementUpdate()
    {

    }

    //protected virtual void SquadEditorUpdate()
    //{

    //}

    protected virtual void PausedUpdate()
    {

    }

    protected void LateUpdate()
    {
        switch (PlayMode)
        {
            case PlayMode.Battle:
                BattleLateUpdate();
                break;

            case PlayMode.UnitPlacement:
                UnitPlacementLateUpdate();
                break;

            //case PlayMode.SquadEditor:
            //    SquadEditorLateUpdate();
            //    break;

            case PlayMode.Paused:
                PausedLateUpdate();
                break;

            default:
                return;
        }
    }

    protected virtual void BattleLateUpdate()
    {
        UnitManager.LateUpdate();
    }

    protected virtual void UnitPlacementLateUpdate()
    {

    }

    //protected virtual void SquadEditorLateUpdate()
    //{

    //}

    protected virtual void PausedLateUpdate()
    {

    }
    #endregion

    #region PlayMode Changes
    public virtual void EnterPlayMode(PlayMode mode)
    {
        if (PlayMode == mode)
        {
            Debug.Log($"Already in {mode}.");
            return;
        }

        PlayMode = mode;
        PlayModeChangedEvent?.Invoke(PlayMode);
    }

    protected abstract void OnBattleComplete(Faction winner);
    #endregion

    #region Board Squares Data
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
            for (int y = -Mathf.CeilToInt(range); y <= Mathf.CeilToInt(range); y++)
            {
                if (x * x + y * y > range * range)
                {
                    continue;
                }

                BoardSquare square = GetSquareAt(center + new Vector2Int(x, y));
                if (square == null)
                {
                    continue;
                }

                squares.Add(square);
            }
        }

        if (squares.Count == 0)
        {
            return null;
        }

        return squares;
    }
    BoardGraph m_moveGraph;
    public Path_AStar<BoardSquare> GetPath(BoardSquare start, bool flying, Func<BoardSquare, bool> endCondition, Func<BoardSquare, float> heuristic = null)
    {
        if (flying)
        {
            return (Path_AStar<BoardSquare>)pathManager.GetPath(m_moveGraph.Flying, start, endCondition, heuristic);
        }

        return (Path_AStar<BoardSquare>)pathManager.GetPath(m_moveGraph.Walking, start, endCondition, heuristic);
    }
    void RefreshMoveGraph()
    {
        m_moveGraph = new BoardGraph(Squares);
    }

    protected virtual void PositionCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = Squares.GetLength(0) / 2f;
        Camera.main.transform.position = cameraPos;
    }
    public bool SetupSquares(int width, int height, bool keepUnits = false)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError($"Invalid board size. Size: {width}, {height}");
            return false;
        }

        Squad units = new Squad();
        if (keepUnits && Squares != null)
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

                }
                else
                {
                    newSquare = Instantiate(m_blackSquare, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<BoardSquare>();
                }
                newSquare.Position = new Vector2Int(x, y);
                Squares[x, y] = newSquare;
            }
        }

        if (keepUnits)
        {
            TryPlaceSquad(units, Vector2Int.zero);
        }

        PositionCamera();
        RefreshMoveGraph();
        return true;
    }
    public void SetSquaresInteractable(int columns, int rows)
    {
        for (int x = 0; x < Squares.GetLength(0); x++)
        {
            for (int y = 0; y < Squares.GetLength(1); y++)
            {
                Squares[x, y].Interactable = x < columns && y < rows;
            }
        }
    }

    protected void ClearBoard()
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

                ClearSquare(Squares[x, y]);
                //Squares[x, y].Clear();
                Destroy(Squares[x, y].gameObject);
                Squares[x, y] = null;
            }
        }
    }
    protected void ClearSquare(BoardSquare square)
    {
        if (square.Unit != null)
        {
            UnitManager.RemoveUnit(square.Unit);
        }

        //Destroy(square.gameObject);
        //Squares[square.Position.x, square.Position.y] = null;
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

                ClearSquare(Squares[x, y]);
            }
        }
    }
    #endregion

    #region Placing and moving units
    public Squad GetBoardAsSquad(bool mirrorBoard = false)
    {
        Vector2Int origin = mirrorBoard ? MirrorPosition(Vector2Int.zero) : Vector2Int.zero;
        ////List<UnitData> units = new List<UnitData>();
        //List<UnitPositionPair> units = new List<UnitPositionPair>();
        //foreach (Unit unit in UnitManager.Units)
        //{
        //    //UnitData newUnit = new UnitData(unit, origin);
        //    UnitData newUnit = new UnitData(unit);

        //    //if (mirrorBoard)
        //    //{
        //    //    newUnit.Position = new Vector2Int(Squares.GetLength(0), Squares.GetLength(1)) - newUnit.Position;
        //    //}
        //    units.Add(new UnitPositionPair(newUnit, unit.Square.Position));
        //}

        //return new Squad(units, origin);
        return new Squad(UnitManager.Units, origin);
    }
    public bool TryMoveUnitTo(Unit unit, BoardSquare target)
    {
        if (target == null || unit == null)
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

    public Unit TryPlaceUnit(UnitData data, Vector2Int position)
    {
        if (position.x < 0 || position.x >= Squares.GetLength(0) ||
            position.y < 0 || position.y >= Squares.GetLength(1) ||
            Squares[position.x, position.y].gameObject.activeInHierarchy == false ||
            Squares[position.x, position.y].Unit != null)
        {
            Debug.LogWarning("Failed to place unit. Invalid data.");
            return null;
        }

        UnitPrototype proto = UnitPrototypeDB.GetProto(data.Type);
        if (proto == null)
        {
            Debug.LogWarning("Failed to place unit. Prototype not found.");
            return null;
        }
        //UnitPrototype proto = UnitManager.GetUnitPrototypeOfType(data.Type);
        //GameObject prefab = UnitManager.GetPrefabOfType(data.Type);
        GameObject prefab = proto.Prefab;
        if (prefab == null)
        {
            Debug.LogWarning("Failed to place unit. Prefab not found.");
            return null;
        }

        Unit unit = Instantiate(prefab, new Vector3(position.x, 0, position.y), Quaternion.identity).GetComponent<Unit>();
        unit.Initialize(this, Squares[position.x, position.y], data, proto);
        return unit;
    }

    public bool TryPlaceSquad(Squad data, Vector2Int offset, bool mirror = false)
    {
        if (data.Units == null ||
            data.Units.Count == 0)
        {
            Debug.Log("No units");
            return false;
        }

        //data.RecalculateParameters();
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
        foreach (Squad.UnitPositionPair pair in data.Units)
        {
            UnitData clone = pair.Unit.Clone();
            Vector2Int position = pair.Position + offset;
            //clone.Position += data.SquadOrigin + offset;
            if (mirror)
            {
                //clone.Position = MirrorPosition(clone.Position);
                position = MirrorPosition(position);
            }
            Unit placedUnit = TryPlaceUnit(clone, position);
            if (placedUnit == null)
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

    Vector2Int MirrorPosition(Vector2Int pos)
    {
        return new Vector2Int(Squares.GetLength(0) - 1, Squares.GetLength(1) - 1) - pos;
    }
    #endregion
}
