using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool IsMet(Unit unit, Board_Base board);
}
