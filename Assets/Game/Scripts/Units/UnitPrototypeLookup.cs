using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPrototypeLookup", menuName = "ScriptableObject/UnitPrototypeLookup", order = 82)]
public class UnitPrototypeLookup : ScriptableObject
{
    [SerializeField] List<UnitPrototype> Protos;

    Dictionary<string, UnitPrototype> m_IDProtoMap;
    static UnitPrototypeLookup m_instance;

    private void OnValidate()
    {
        SetupProtoMap();
    }

    void SetupProtoMap()
    {
        if (Protos == null)
        {
            Protos = new List<UnitPrototype>();
        }

        if (m_IDProtoMap == null)
        {
            m_IDProtoMap = new Dictionary<string, UnitPrototype>();
        }

        foreach (UnitPrototype proto in Protos)
        {
            m_IDProtoMap[proto.Type] = proto;
        }
    }

    public static UnitPrototype GetProto(string type)
    {
        if(m_instance == null)
        {
            m_instance = Resources.Load<UnitPrototypeLookup>("UnitPrototypeLookup");
            m_instance.SetupProtoMap();
        }

        if(!m_instance.m_IDProtoMap.ContainsKey(type))
        {
            Debug.LogWarning($"Unit prototype with ID {type} not found.");
            return null;
        }

        return m_instance.m_IDProtoMap[type];
    }

    public static void AddPrototype(UnitPrototype proto)
    {
        if (m_instance == null)
        {
            m_instance = Resources.Load<UnitPrototypeLookup>("UnitPrototypeLookup");
            m_instance.SetupProtoMap();
        }

        if (m_instance.Protos.Contains(proto))
        {
            Debug.LogError($"Duplicate unit type: {proto.Type}");
            return;
        }

        m_instance.Protos.Add(proto);
        m_instance.SetupProtoMap();
    }
}
