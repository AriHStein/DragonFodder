using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadPrototypeLookup", menuName = "ScriptableObject/SquadPrototypeLookup", order = 82)]
public class SquadPrototypeDB : ScriptableObject
{
    [SerializeField] List<SquadPrototype> Protos;

    Dictionary<string, SquadPrototype> m_map;
    static SquadPrototypeDB m_instance;

    private void OnValidate()
    {
        Init();
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        m_instance = Resources.LoadAll<SquadPrototypeDB>("UnitDBs")[0];
        m_instance.SetupProtoMap();

        //if (m_instance == null)
        //{
        //    m_instance = Resources.Load<SquadPrototypeLookup>("SquadPrototypeLookup");
        //    m_instance.SetupProtoMap();
        //}
    }

    void SetupProtoMap()
    {
        if (Protos == null)
        {
            Protos = new List<SquadPrototype>();
        }

        if (m_map == null)
        {
            m_map = new Dictionary<string, SquadPrototype>();
        }

        foreach (SquadPrototype proto in Protos)
        {
            m_map[proto.Type] = proto;
        }
    }

    public static SquadPrototype GetProto(string type)
    {
        if (m_instance == null)
        {
            Init();
        }
        //Initialize();

        if (!m_instance.m_map.ContainsKey(type))
        {
            Debug.LogWarning($"Unit prototype with ID {type} not found.");
            return null;
        }

        return m_instance.m_map[type];
    }

    public static List<SquadPrototype> GetPrototypes(System.Func<SquadPrototype, bool> criteria)
    {
        if(criteria == null)
        {
            return GetAllPrototypes();
        }

        if (m_instance == null)
        {
            Init();
        }
        //Initialize();

        List<SquadPrototype> protos = new List<SquadPrototype>();
        foreach (SquadPrototype proto in m_instance.m_map.Values)
        {
            if (criteria(proto))
            {
                protos.Add(proto);
            }
        }

        return protos;
    }

    public static List<SquadPrototype> GetAllPrototypes()
    {
        //Initialize();

        return new List<SquadPrototype>(m_instance.m_map.Values);
    }

    public static void AddPrototype(SquadPrototype proto)
    {
        if(m_instance == null)
        {
            Init();
        }
        //Initialize();

        if (m_instance.Protos.Contains(proto))
        {
            return;
        }

        m_instance.Protos.Add(proto);
        m_instance.SetupProtoMap();
    }
}
