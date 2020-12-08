using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Encounter : ISerializationCallbackReceiver
{
    public SerializableGuid ID;
    
    public Vector2Int MapPosition;
    [NonSerialized]
    public List<Encounter> Connections;
    public List<SerializableGuid> ConnectionIDs; // For serialization
    
    public Vector2Int BoardSize;
    public SquadData Enemies;
    public int RowsAllowedForPlayerUnits;

    public int Reward;

    public bool Complete;
    public bool Available;

    // Constructor for new Encounters
    public Encounter(Vector2Int mapPosition, SquadData enemies, Vector2Int boardSize, int playerRows, int reward)
    {
        ID = Guid.NewGuid();        
        MapPosition = mapPosition;

        Enemies = enemies;
        BoardSize = boardSize;
        RowsAllowedForPlayerUnits = playerRows;
        Reward = reward;
        Complete = false;
        Available = false;

        Connections = new List<Encounter>();
        //ConnectionIDs = new List<SerializableGuid>();
    }

    public void OnBeforeSerialize()
    {
        if(ConnectionIDs == null)
        {
            Debug.Log($"OnBeforeSerialize. Connection count: {Connections.Count}");
            ConnectionIDs = new List<SerializableGuid>();
            foreach (Encounter encoutner in Connections)
            {
                ConnectionIDs.Add(encoutner.ID);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        if(Connections == null)
        {
            Connections = new List<Encounter>();
            Debug.Log($"OnAfterDeserialize. Connection count: {ConnectionIDs.Count}");
        }
    }

    //// Constructor for deserializing encounters
    //public Encounter(Guid id, Vector2Int mapPosition, List<SerializableGuid> connections, SquadData enemies, Vector2Int boardSize, int playerRows, int reward, bool complete)
    //{
    //    if(id == Guid.Empty)
    //    {
    //        Debug.LogError("id == Guid.Empty. Assigning a new Guid.");
    //        ID = Guid.NewGuid();
    //    }
    //    else
    //    {
    //        ID = id;
    //    }

    //    MapPosition = mapPosition;

    //    if(connections == null || connections.Count == 0)
    //    {
    //        Debug.LogError("Encounter has no connections.");
    //        ConnectionIDs = new List<SerializableGuid>();
    //    } 
    //    else
    //    {
    //        ConnectionIDs = connections;
    //    }
        
    //    Enemies = enemies;
    //    BoardSize = boardSize;
    //    RowsAllowedForPlayerUnits = playerRows;
    //    Reward = reward;
    //    Complete = complete;

    //    Connections = new List<Encounter>();
    //}

    public void ConnectTo(Encounter other)
    {
        if(other == null || other == this)
        {
            Debug.LogError("Invalid connection.");
            return;
        }

        if(Connections.Contains(other))
        {
            Debug.LogWarning("Already connected to other encounter.");
            return;
        }

        Connections.Add(other);

        //if(!ConnectionIDs.Contains(other.ID))
        //    ConnectionIDs.Add(other.ID);
    }

    public void FormAllConnections(Dictionary<Guid, Encounter> encounters) 
    {
        if(encounters == null)
        {
            Debug.LogError("encounters is null!");
            return;
        }
        
        foreach(Guid id in ConnectionIDs)
        {
            if(!encounters.ContainsKey(id))
            {
                Debug.LogWarning($"Encounter {id} not found!");
                continue;
            }

            if(encounters[id] == null)
            {
                Debug.LogError($"Encounter {id} is null!");
                continue;
            }

            ConnectTo(encounters[id]);
        }
    }
}
