using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager
{
    public List<Unit> Units { get; protected set; }
    public List<UnitData> UnplacedUnits;
    HashSet<Unit> m_diedThisTurn;

    Dictionary<Faction, int> m_factionCount;

    public event Action<Faction> BattleCompleteEvent;

    public UnitManager()
    {
        Units = new List<Unit>();
        UnplacedUnits = new List<UnitData>();
        m_diedThisTurn = new HashSet<Unit>();
        m_newUnitsThisTurn = new HashSet<Unit>();
        m_factionCount = new Dictionary<Faction, int>();
    }

    HashSet<Unit> m_newUnitsThisTurn;
    public void RegisterUnit(Unit unit)
    {
        m_newUnitsThisTurn.Add(unit);
    }

    void AddUnit(Unit unit) 
    {
        Units.Add(unit);
        unit.DeathEvent += OnUnitDeath;

        if (!m_factionCount.ContainsKey(unit.Faction))
        {
            m_factionCount[unit.Faction] = 1;
        }
        else
        {
            m_factionCount[unit.Faction]++;
        }
    }

    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
        unit.Square.Unit = null;
        unit.Square = null;
        if(m_factionCount.ContainsKey(unit.Faction) && m_factionCount[unit.Faction] > 0)
        {
            m_factionCount[unit.Faction]--;
        } else
        {
            Debug.LogWarning($"Removing unit of faction {unit.Faction}, but no units of this faction have been registered!");
        }

        GameObject.Destroy(unit.gameObject);
    }

    public void DoUnitTurns(float deltaTime, Board_Base board)
    {
        m_diedThisTurn.Clear();
        foreach (Unit unit in Units)
        {
            if(unit.ReadyForTurn(deltaTime))
            {
                unit.DoTurn(board);
            }
        }
    }

    void OnUnitDeath(Unit unit)
    {
        m_diedThisTurn.Add(unit);
    }

    public void LateUpdate()
    {
        foreach(Unit unit in m_diedThisTurn)
        {
            RemoveUnit(unit);
        }
        m_diedThisTurn.Clear();

        foreach (Unit unit in m_newUnitsThisTurn)
        {
            AddUnit(unit);
        }
        m_newUnitsThisTurn.Clear();

        foreach(Faction faction in m_factionCount.Keys)
        {
            if (m_factionCount[faction] == 0)
            {
                BattleCompleteEvent?.Invoke(faction.Opposite());
                return;
            }
        }

    }

    public void TriggerVictoryAnimations()
    {
        foreach(Unit unit in Units)
        {
            unit.VictoryDance();
        }
    }

    public List<Unit> GetUnitsOfFaction(Faction faction)
    {
        if(!m_factionCount.ContainsKey(faction) || m_factionCount[faction] == 0)
        {
            return null;
        }

        List<Unit> units = new List<Unit>();
        foreach(Unit unit in Units)
        {
            if(unit.Faction == faction)
            {
                units.Add(unit);
            }
        }

        return units;
    }

    public Unit GetNearestUnitOfFaction(Faction faction, BoardSquare origin)
    {
        if(origin == null)
        {
            Debug.LogError("Origin is null.");
            return null;
        }
        
        Unit nearest = null;
        float nearestDistance = float.MaxValue;
        foreach(Unit unit in Units)
        {
            if(unit.Faction != faction)
            {
                continue;
            }

            float distance = Vector2Int.Distance(origin.Position, unit.Square.Position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = unit;
            }
        }

        return nearest;
    }

    public Unit GetFarthestUnitOfFaction(Faction faction, BoardSquare origin)
    {
        if (origin == null)
        {
            Debug.LogError("Origin is null.");
            return null;
        }

        Unit farthest = null;
        float farthestDistance = 0f;
        foreach (Unit unit in Units)
        {
            if (unit.Faction != faction)
            {
                continue;
            }

            float distance = Vector2Int.Distance(origin.Position, unit.Square.Position);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthest = unit;
            }
        }

        return farthest;
    }
}
