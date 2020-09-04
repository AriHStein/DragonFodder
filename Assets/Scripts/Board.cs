using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Current;
    public UnitManager UnitManager { get; protected set; }

    [SerializeField] GameObject m_whiteSquare = default;
    [SerializeField] GameObject m_blackSquare = default;
    [SerializeField] int m_width = 8;
    [SerializeField] int m_height = 8;

    [SerializeField] List<UnitData> m_units = default;

    [SerializeField] float m_timeBetweenTurns = 2f;
    float m_timeUntilNextTurn;

    //public int width;
    //public int height;
    //public List<BoardSquare> Squares;
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

        m_timeUntilNextTurn = m_timeBetweenTurns;
        TimeManager.Pause();

        SetupSquares();
        SetupUnits();

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = m_width / 2f;
        Camera.main.transform.position = cameraPos;
    }

    private void Start()
    {

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
                newSquare.Pos = new Vector2Int(x, y);
                Squares[x, y] = newSquare;
            }
        }
    }

    void SetupUnits()
    {
        foreach(UnitData data in m_units)
        {
            Unit unit = Instantiate(data.Prefab,  new Vector3(data.Position.x, 0, data.Position.y), Quaternion.identity).GetComponent<Unit>();
            Squares[data.Position.x, data.Position.y].Unit = unit;
            unit.Square = Squares[data.Position.x, data.Position.y];
            unit.Faction = data.Faction;
        }
    }

    private void Update()
    {
        if(TimeManager.Paused)
        {
            return;
        }

        if(m_timeUntilNextTurn <= 0)
        {
            UnitManager.DoUnitTurns();
            m_timeUntilNextTurn = m_timeBetweenTurns;
        } else
        {
            m_timeUntilNextTurn -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        UnitManager.CleanupDeadUnits();
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
}
