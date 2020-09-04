using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Current;
    public UnitManager UnitManager { get; protected set; }

    [SerializeField] float m_timeBetweenTurns = 2f;
    float m_timeUntilNextTurn;

    public int width;
    public int height;
    public List<BoardSquare> Squares;
    public BoardSquare GetSquareAt(int row, int column)
    {
        if(row >= height || column >= width || row < 0 || column < 0)
        {
            return null;
        }
        
        return Squares[row * width + column];
    }


    public void Awake()
    {
        UnitManager = new UnitManager();
        Current = this;

        m_timeUntilNextTurn = m_timeBetweenTurns;
        TimeManager.Pause();
    }

    private void Start()
    {
        for(int i = 0; i<Squares.Count; i++)
        {
            Squares[i].Pos = new Vector2Int(i/width, i %width);
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
