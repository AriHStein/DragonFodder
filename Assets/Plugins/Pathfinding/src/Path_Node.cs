using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority_Queue;

namespace Pathfinding
{
    
    public class Path_Node<T> : FastPriorityQueueNode
    {

        public T data;                      // Generic data class -- nodes for the base use case.
        public List<Path_Edge<T>> edges;    // Nodes leading OUT from this node

        public Path_Node()
        {
            edges = new List<Path_Edge<T>>();
        }

        public Path_Node(T data)
        {
            this.data = data;
            edges = new List<Path_Edge<T>>();
        }

        public Path_Node(T data, List<Path_Edge<T>> edges)
        {
            this.data = data;
            this.edges = edges;
        }

        public Path_Edge<T> EdgeTo(Path_Node<T> dest)
        {
            foreach (Path_Edge<T> edge in edges)
            {
                if (edge.node == dest)
                {
                    return edge;
                }
            }

            return null;
        }
    }
}
