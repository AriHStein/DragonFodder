using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public class PathManager_AStar<T> : IPathManager<T>
    {
        public IPath<T> GetPath(IPath_Graph<T> graph, T startNode, Func<T, bool> endCond, Func<T, float> heuristic = null)
        {
            try
            {
                return new Path_AStar<T>(graph, startNode, endCond, heuristic);
            } catch(PathNotFoundException)
            {
                return null;
            }
        }
    }
}
