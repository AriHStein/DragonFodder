using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityContext
{
    Unit Actor { get; }
    int Value { get; }
}
