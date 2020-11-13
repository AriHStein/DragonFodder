using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityTree", menuName = "Units/Progression/AbilityTree")]
public class AbilityTree : ScriptableObject
{
    [SerializeField] List<AbilityTreeNode> m_baseAbilities = default;
    
    public List<AbilityPrototype> GetAbilities()
    {
        List<AbilityPrototype> abilities = new List<AbilityPrototype>();
        foreach(AbilityTreeNode node in m_baseAbilities)
        {
            AddAbility(node, ref abilities);
        }

        return abilities;
    }

    void AddAbility(AbilityTreeNode node, ref List<AbilityPrototype> abilities)
    {
        if(node.Unlocked)
        {
            if(node.Replaces != null && abilities.Contains(node.Replaces))
            {
                abilities[abilities.IndexOf(node.Replaces)] = node.Ability;
            }
            else
            {
                abilities.Add(node.Ability);
            }

            foreach(AbilityTreeNode child in node.Children)
            {
                AddAbility(child, ref abilities);
            }
        }
    }
}
