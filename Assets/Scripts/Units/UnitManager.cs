using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    List<Unit> m_units;
    HashSet<Unit> m_diedThisTurn;

    Dictionary<Faction, int> m_factionCount;
    
    public UnitManager()
    {
        m_units = new List<Unit>();
        m_diedThisTurn = new HashSet<Unit>();
        m_factionCount = new Dictionary<Faction, int>();
    }

    public void RegisterUnit(Unit unit)
    {
        //Debug.Log($"{unit.name} registered");
        m_units.Add(unit);
        unit.DeathEvent += OnUnitDeath;

        if(!m_factionCount.ContainsKey(unit.Faction))
        {
            m_factionCount[unit.Faction] = 1;
        } else
        {
            m_factionCount[unit.Faction]++;
        }
    }


    public void DoUnitTurns()
    {
        m_diedThisTurn.Clear();
        foreach(Unit unit in m_units)
        {
            unit.DoTurn();
        }
    }

    void OnUnitDeath(Unit unit)
    {
        m_diedThisTurn.Add(unit);
        //m_units.Remove(unit);
    }

    public void CleanupDeadUnits()
    {
        foreach(Unit unit in m_diedThisTurn)
        {
            //Debug.Log($"{unit.name} died");
            m_units.Remove(unit);
            unit.Square.Unit = null;
            unit.Square = null;
            m_factionCount[unit.Faction]--;
            if(m_factionCount[unit.Faction] == 0)
            {
                Board.Current.EnterUnitPlacementMode();
            }
            GameObject.Destroy(unit.gameObject);
        }

        m_diedThisTurn.Clear();
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
        foreach(Unit unit in m_units)
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
}
