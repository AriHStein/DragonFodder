using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public interface IPathManager<T>
    {
        IPath<T> GetPath(IPath_Graph<T> graph, T start, Func<T, bool> endCondition, Func<T, float> heuristic = null);

    }
}
