using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Path_Edge<T>
    {

        public Path_Node<T> node;       // Endpoint node of this edge
        public float cost;              // cost of traversing this edge


        public Path_Edge(Path_Node<T> node, float cost)
        {
            this.node = node;
            this.cost = cost;
        }
    }
}

