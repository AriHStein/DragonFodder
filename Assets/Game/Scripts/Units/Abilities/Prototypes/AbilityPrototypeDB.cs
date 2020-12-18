using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityPrototypeLookup", menuName = "ScriptableObject/AbilityPrototypeLookup", order = 82)]
public class AbilityPrototypeDB : ScriptableObject
{
    [SerializeField] List<AbilityPrototype> AbilityTypes;

    Dictionary<string, AbilityPrototype> m_map;
    static AbilityPrototypeDB m_instance;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        m_instance = Resources.LoadAll<AbilityPrototypeDB>("UnitDBs")[0];
        m_instance.SetupMap();
    }

    private void OnValidate()
    {
        SetupMap();
    }

    void SetupMap()
    {
        if (AbilityTypes == null)
        {
            AbilityTypes = new List<AbilityPrototype>();
        }

        if (m_map == null)
        {
            m_map = new Dictionary<string, AbilityPrototype>();
        }

        foreach (AbilityPrototype abilityType in AbilityTypes)
        {
            m_map[abilityType.GetType().Name] = abilityType;
        }
    }

    public static AbilityPrototype GetAbilityOfType(string type)
    {
        if (m_instance == null)
        {
            Init();
            //m_instance = Resources.Load<AbilityPrototypeDB>("AbilityLookup");
            //m_instance.SetupMap();
        }

        if (!m_instance.m_map.ContainsKey(type))
        {
            Debug.LogWarning($"Ability {type} not found.");
            return null;
        }

        return ScriptableObject.CreateInstance(type) as AbilityPrototype;
    }

    public static List<string> GetAbilityTypes()
    {
        if (m_instance == null)
        {
            Init();
            //m_instance = Resources.Load<AbilityPrototypeDB>("AbilityPrototypeLookup");
            //m_instance.SetupMap();
        }

        return new List<string>(m_instance.m_map.Keys);
    }
}
