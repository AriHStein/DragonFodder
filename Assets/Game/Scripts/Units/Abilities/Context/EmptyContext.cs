using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyContext : IAbilityContext
{
    public Unit Actor { get { return null; } }
    public int Value { get { return -1; } }
}
