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
        if (Connections == null)
        {
            Connections = new List<Encounter>();
        }

        if (ConnectionIDs == null)
        {
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
        }
    }

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
