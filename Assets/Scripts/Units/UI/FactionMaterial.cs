using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Faction Material", menuName = "Units/FactionMaterial", order = 112)]
public class FactionMaterial : ScriptableObject
{
    public Faction Faction;
    public Material Material;
}
