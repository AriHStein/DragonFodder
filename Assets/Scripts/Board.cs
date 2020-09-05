using System.Collections.Generic;
using UnityEngine;

public enum PlayMode { UnitPlacement, Battle }
public class Board : MonoBehaviour
{
    public static Board Current;
    public PlayMode PlayMode { get; protected set; }
    public UnitManager UnitManager { get; protected set; }

    [SerializeField] GameObject m_unitPlacementModePanel = default;

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] int m_width = 8;
    [SerializeField] int m_height = 8;

    [SerializeField] List<UnitData> m_preplacedUnits = default;

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
        UnitManager = new UnitManager();
        Current = this;
        PlayMode = PlayMode.UnitPlacement;

        m_timeUntilNextTurn = m_timeBetweenTurns;

        SetupSquares();
        SetupUnits();

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = m_width / 2f;
        Camera.main.transform.position = cameraPos;
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
        foreach(UnitData data in m_preplacedUnits)
        {
            //Unit unit = Instantiate(data.Prefab,  new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
            //unit.Initialize(Squares[data.Position.x, data.Position.y], data);
            TryPlaceNewUnit(data);
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

    public void EnterBattleMode()
    {
        PlayMode = PlayMode.Battle;
    }

    public void EnterUnitPlacementMode()
    {
        PlayMode = PlayMode.UnitPlacement;
        m_unitPlacementModePanel.SetActive(true);
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
            Squares[data.Position.x, data.Position.y].Unit != null ||
            data.Prefab == null || data.Prefab.GetComponent<Unit>() == null)
        {
            return false;
        }
        
        Unit unit = Instantiate(data.Prefab, new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
        unit.Initialize(Squares[data.Position.x, data.Position.y], data);
        return true;
    }
}
