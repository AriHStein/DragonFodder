using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityTree", menuName = "Units/Progression/AbilityTreeNode")]
public class AbilityTreeNode : ScriptableObject
{
    public AbilityPrototype Ability;
    public bool Unlocked;

    public AbilityPrototype Replaces;

    public List<AbilityTreeNode> Children;
}
