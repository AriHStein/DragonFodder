using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RecruitableUnitCollection
{
    //public string Type;
    public UnitPrototype Proto;
    [SerializeField, HideInInspector] private string m_type;
    public int Count;

    public RecruitableUnitCollection(UnitPrototype proto, int count)
    {
        Proto = proto;
        Count = count;
    }
}
