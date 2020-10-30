using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Vulnerable", menuName ="Units/Statuses/Vulnerable", order =125)]
public class Vulnerable : Status
{
    public override string Type { get { return "Vunlerable"; } }

    [SerializeField] float m_damageMultiplier = 1.5f;

    public override int ModifyDamageRecieved(int damage)
    {
        return Mathf.RoundToInt(damage * m_damageMultiplier);
    }
}
