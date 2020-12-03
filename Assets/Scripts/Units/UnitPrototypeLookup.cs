using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitPrototypeLookup", menuName = "UnitPrototypeLookup")]
public class UnitPrototypeLookup : ScriptableObject
{
    [SerializeField] List<UnitPrototype> Protos;

    Dictionary<string, UnitPrototype> m_IDProtoMap;

    private void OnValidate()
    {
        if(Protos == null)
        {
            Protos = new List<UnitPrototype>();
        }

        if(m_IDProtoMap == null)
        {
            m_IDProtoMap = new Dictionary<string, UnitPrototype>();
        }

        foreach(UnitPrototype proto in Protos)
        {
            m_IDProtoMap[proto.Type] = proto;
        }
    }

    public UnitPrototype GetProto(string type)
    {
        if(m_IDProtoMap == null)
        {
            return null;
        }

        if(!m_IDProtoMap.ContainsKey(type))
        {
            Debug.LogWarning($"Unit prototype with ID {type} not found.");
            return null;
        }

        return m_IDProtoMap[type];
    }
}
