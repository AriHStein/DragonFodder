using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Stun Status", menuName ="Units/Statuses/Stun", order = 125)]
public class Stun : Status
{
    public override string Type { get { return "Stun"; } }
    //[SerializeField] float m_duration;

    //public Stun(float duration)
    //{
    //    m_duration = duration;
    //}

    public override bool ReadyForTurn(float deltaTime)
    {
        base.ReadyForTurn(deltaTime);

        return Duration <= 0;
    }
}
