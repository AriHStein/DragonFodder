using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBase : Unit
{
    [SerializeField] float m_range = 1f;
    [SerializeField] float m_moveSpeed = 2f;

    Unit m_target;
    
    public override void DoTurn()
    {
        //Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        //Unit target = Board.Current.UnitManager.GetFarthestUnitOfFaction(targetFaction, Square);

        //if (target == null)
        //{
        //    Debug.LogWarning("No Target found");
        //    return;
        //}

        if(m_target == null)
        {
            ChooseTarget();
            if(m_target == null)
            {
                return;
            }
        }

        if (Vector2Int.Distance(m_target.Square.Position, Square.Position) <= m_range)
        {
            FaceToward(m_target.Square);
            Attack(m_target);
        }
        else
        {
            MoveToward(m_target.Square);
            FaceToward(m_target.Square);
        }
    }

    void ChooseTarget()
    {
        Faction targetFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
        m_target = Board.Current.UnitManager.GetFarthestUnitOfFaction(targetFaction, Square);

        if (m_target == null)
        {
            Debug.LogWarning("No Target found");
            return;
        }

        m_target.DeathEvent += OnTargetDeath;
    }

    void OnTargetDeath(Unit unit)
    {
        if(m_target != unit)
        {
            Debug.LogWarning("Dead unit is not m_target");
            return;
        }

        m_target = null;
    }

    protected override bool MoveToward(BoardSquare dest)
    {
        Vector2Int move = dest.Position - Square.Position;
        Vector2Int destPos;
        if(move.magnitude <= m_moveSpeed)
        {
            destPos = dest.Position;
            //MoveToNearestOpenAdjacent(dest);
        } else
        {
            Vector2 movef = move;
            movef = movef.normalized * m_moveSpeed;
            destPos = new Vector2Int(Mathf.FloorToInt(movef.x), Mathf.FloorToInt(movef.y)) + Square.Position;
            //MoveToNearestOpenAdjacent(Board.Current.GetSquareAt(destPos.x, destPos.y));
        }

        return MoveToOpenAdjacent(Board.Current.GetSquareAt(destPos.x, destPos.y), move);
    }

    bool MoveToOpenAdjacent(BoardSquare dest, Vector2Int fromDir)
    {
        BoardSquare moveTo = null;
        if(CanMoveTo(dest))
        {
            moveTo = dest;
        } else
        {
            // check all squares adjacent to the dest for an open square. Start with squares closest to current position.
            Vector2Int[] directions = new Vector2Int[4];
            if (Mathf.Abs(fromDir.x) > Mathf.Abs(fromDir.y))
            {
                directions[0] = new Vector2Int(-(int)Mathf.Sign(fromDir.x), 0);
                directions[1] = new Vector2Int(0, -(int)Mathf.Sign(fromDir.y));
                directions[2] = new Vector2Int((int)Mathf.Sign(fromDir.x), 0);
                directions[3] = new Vector2Int(0, (int)Mathf.Sign(fromDir.y));
            } else
            {
                directions[0] = new Vector2Int(0, -(int)Mathf.Sign(fromDir.y));
                directions[1] = new Vector2Int(-(int)Mathf.Sign(fromDir.x), 0);
                directions[2] = new Vector2Int(0, (int)Mathf.Sign(fromDir.y));
                directions[3] = new Vector2Int((int)Mathf.Sign(fromDir.x), 0);
            }

            BoardSquare checkSquare;
            for(int i = 0; i < directions.Length; i++)
            {
                checkSquare = Board.Current.GetSquareAt(dest.Position + directions[i]);
                if(CanMoveTo(checkSquare))
                {
                    moveTo = checkSquare;
                    break;
                }
            }

            // No squares adjacent to target were open. Do not move.
            //return false;
        }

        return Board.Current.TryMoveUnitTo(this, moveTo);
    }

    bool CanMoveTo(BoardSquare dest)
    {
        return dest != null && (dest.Unit == null || dest.Unit == this);
    }
}
