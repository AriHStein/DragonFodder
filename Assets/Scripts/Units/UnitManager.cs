using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager
{
    public List<Unit> Units { get; protected set; }
    HashSet<Unit> m_diedThisTurn;

    //List<Unit> m_turnOrder;

    Dictionary<string, UnitPrototype> m_unitPrototypeMap;
    //Dictionary<string, GameObject> m_unitPrefabMap;
    Dictionary<Faction, int> m_factionCount;

    public event Action<Faction> BattleWonEvent;

    public UnitManager(List<UnitPrototype> prototypes)
    {
        Units = new List<Unit>();
        //m_turnOrder = new List<Unit>();
        m_diedThisTurn = new HashSet<Unit>();
        m_newUnitsThisTurn = new HashSet<Unit>();
        m_factionCount = new Dictionary<Faction, int>();

        //m_unitPrefabMap = new Dictionary<string, GameObject>();
        m_unitPrototypeMap = new Dictionary<string, UnitPrototype>();
        foreach(UnitPrototype proto in prototypes)
        {
            //Unit unit = go.GetComponent<Unit>();
            //if(unit == null)
            //{
            //    Debug.LogWarning($"UnitPrefab {go.name} did not have a Unit component attached.");
            //    continue;
            //}

            //if (unit.Proto == null)
            //{
            //    Debug.LogError($"Unit {go.name}'s Prototype is null.");
            //    //continue;
            //}

            //m_unitPrefabMap[unit.Type] = go;
            m_unitPrototypeMap[proto.Type] = proto;
        }
    }

    public UnitPrototype GetUnitPrototypeOfType(string type)
    {
        if (type == null || !m_unitPrototypeMap.ContainsKey(type))
        {
            Debug.LogWarning($"Prototype for unit of type {type} not found.");
            return null;
        }

        return m_unitPrototypeMap[type];
    }

    //public GameObject GetPrefabOfType(string type)
    //{
    //    if(type == null || !m_unitPrefabMap.ContainsKey(type))
    //    {
    //        Debug.LogWarning($"Prefab for unit of type {type} not found.");
    //        return null;
    //    }

    //    return m_unitPrefabMap[type];
    //}

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
        //m_turnOrder.Remove(unit);
        unit.Square.Unit = null;
        unit.Square = null;
        //Debug.Log(unit.Faction);
        if(m_factionCount.ContainsKey(unit.Faction) && m_factionCount[unit.Faction] > 0)
        {
            m_factionCount[unit.Faction]--;
        } else
        {
            Debug.LogWarning($"Removing unit of faction {unit.Faction}, but no units of this faction have been registered!");
        }

        GameObject.Destroy(unit.gameObject);
    }

    //public void SetTurnOrder()
    //{
    //    Debug.Log($"SetTurnOrder. {Units.Count} units");
    //    m_turnOrder = Units.OrderByDescending(u => u.Proto.TimeBetweenActions).ToList();
    //}

    public void DoUnitTurns(float deltaTime, Board board)
    {
        m_diedThisTurn.Clear();
        //if(m_turnOrder.Count == 0)
        //{
        //    SetTurnOrder();
        //}
        //m_turnOrder[m_turnOrder.Count - 1].DoTurn();
        //m_turnOrder.RemoveAt(m_turnOrder.Count - 1);
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
        //m_units.Remove(unit);
    }

    public void LateUpdate()
    {
        foreach(Unit unit in m_diedThisTurn)
        {
            //Debug.Log($"{unit.name} died");
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
                BattleWonEvent?.Invoke(faction.Opposite());
                //Board.Current.ExitBattleMode(faction == Faction.Enemy);
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
