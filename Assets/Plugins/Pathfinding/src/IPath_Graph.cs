using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public interface IPath_Graph<T>
    {
        Dictionary<T, Path_Node<T>> Graph { get; }
    }
}
