using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

namespace Pathfinding
{
    public class Path_AStar<T> : IPath<T>
    {
        public T Dequeue()
        {
            if (path == null || path.Count == 0)
            {
                //Debug.LogError("Attempting to dequeue a node from an empty or null path.");
                return default(T);
            }

            T obj = path[path.Count - 1];
            path.RemoveAt(path.Count - 1);

            return obj;
        }

        public T Peek()
        {
            if (path == null || path.Count == 0)
            {
                return default(T);
            }

            return path[path.Count - 1];
        }

        public int Length()
        {
            if (path == null)
            {
                return 0;
            }

            return path.Count;
        }

        public T End()
        {
            if (path == null || path.Count == 0)
            {
                return default(T);
            }

            return path[0];
        }

        public T Start()
        {
            if (path == null || path.Count == 0)
            {
                return default(T);
            }

            return path[path.Count - 1];
        }

        List<T> path;
        public T this[int i] { get { return path[i]; } }

        Func<T, bool> endCondition;
        Func<T, float> heuristicFunction;

        /// <summary>
        /// Finds a list of nodes that are a path from start node to a node that meets the end condition.
        /// For A* pathfinding (biased scanning toward the destination), use Path_AStar.EuclidianDistance(***, endNode) for the heuristic function.
        /// For Dijkstra's pathfinding (scanning equally in all directions), use 0 as the heuristic function. 
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endCondition"></param>
        /// <param name="heuristicFunction"></param>
        internal Path_AStar(IPath_Graph<T> graph, T startNode, Func<T, bool> endCond, Func<T, float> heuristic = null)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }

            if (startNode == null)
            {
                throw new ArgumentNullException("startNode");
            }

            if (endCond == null)
            {
                throw new ArgumentNullException("endCond");
            }

            endCondition = endCond;
            if (heuristic != null)
            {
                heuristicFunction = heuristic;
            } else
            {
                heuristicFunction = (node) => { return 0; };
            }

            //int checkedNodeCount = 0;

            // cache dictionary of all walkable nodes
            Dictionary<T, Path_Node<T>> nodes = graph.Graph;

            // validate start and end nodes
            if (nodes.ContainsKey(startNode) == false)
            {
                endCondition = null;
                heuristicFunction = null;
                nodes = null;
                throw new ArgumentException("startNode isn't in the node graph!");
            }

            Path_Node<T> start = nodes[startNode];

            // closedSet is the list of nodes that have already been checked
            // openSet is the queue of potentially useful nodes that have not yet been checked
            // when a node is checked, it moves from the open set to the closed set,
            // and its neighbors *may* be added to the open set
            HashSet<Path_Node<T>> closedSet = new HashSet<Path_Node<T>>();
            FastPriorityQueue<Path_Node<T>> openSet = new FastPriorityQueue<Path_Node<T>>(graph.Graph.Keys.Count);
            openSet.Enqueue(start, 0);

            // initialize map of the most efficient node that a node can be reach from
            Dictionary<Path_Node<T>, Path_Node<T>> cameFrom = new Dictionary<Path_Node<T>, Path_Node<T>>(graph.Graph.Keys.Count);

            // initialize g_score map of cost to reach a node from start position
            // initialize f_score map of g_score + heuristic cost estimate to reach end from a given node
            // default value of infinity for all nodes for both scores
            Dictionary<Path_Node<T>, float> g_score = new Dictionary<Path_Node<T>, float>();
            Dictionary<Path_Node<T>, float> f_score = new Dictionary<Path_Node<T>, float>();
            foreach (Path_Node<T> n in nodes.Values)
            //foreach (Node n in nodes.Keys)
            {
                g_score[n] = Mathf.Infinity;
                f_score[n] = Mathf.Infinity;
            }

            g_score[start] = 0;                                 // set starting node g_score
            f_score[start] = heuristicFunction(start.data);     // set starting node f_score

            while (openSet.Count > 0)
            {
                // Since openSet is a priority queue, 
                // the first item should have the lowest f_score.
                // Use that item as current.
                Path_Node<T> current = openSet.Dequeue();
                //checkedNodeCount++;
                //Node current = openSet.Dequeue();

                // Have we met the end condition (and therefore found a valid path)?
                if (endCondition(current.data))
                {
                    ReconstructPath(cameFrom, current);
                    return;
                }

                // We've checked the current node. 
                // Add it to the closed set and check paths leading out of it.
                closedSet.Add(current);
                //if(nodes[current] == null || nodes[current].Count == 0)
                //{
                //    continue; 
                //}

                foreach (Path_Edge<T> edge in current.edges)
                //foreach (Path_Edge<Node> edge in nodes[current])
                {
                    Path_Node<T> neighbor = edge.node;
                    //Node neighbor = edge.node;
                    if (closedSet.Contains(neighbor))
                    {
                        continue; // this neighbor has already been checked. Skip it
                    }

                    //float moveCostToNeighbor = edge.cost * DistanceBetween(current, neighbor);

                    // calc g_score for neighbor ASSUMING you're comming from current
                    //float tentative_g_score = g_score[current] + moveCostToNeighbor;
                    float tentative_g_score = g_score[current] + edge.cost;

                    // skip this neighbor if tentative g_score is worse than current g_score
                    if (openSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor])
                    {
                        continue;
                    }


                    cameFrom[neighbor] = current;               // record path
                    g_score[neighbor] = tentative_g_score;      // set new (best) g_score
                    f_score[neighbor] = g_score[neighbor] +     // set new (best) f_score
                        heuristicFunction(neighbor.data);

                    // if neighbor has not been checked before now, add it to the queue of nodes to check
                    if (openSet.Contains(neighbor) == false)
                    {
                        openSet.Enqueue(neighbor, f_score[neighbor]);
                    } else // if it has been checked, update it's f_score
                    {
                        openSet.UpdatePriority(neighbor, f_score[neighbor]);
                    }
                }
            }

            // if we exited the above while loop, we checked the entire openSet without
            // finding a node where current == end
            // that means that there is no valid path from the start to end
            // return failure state (Path_AStar with path == null)
            //Debug.LogWarning($"No path found. Checked {checkedNodeCount} nodes.");
            throw new PathNotFoundException();
        }

        /// <summary>
        /// Current is the end point
        /// walk backward through Came_From until we reach the end of the map,
        /// which should be the start node
        /// </summary>
        /// <param name="Came_From"></param>
        /// <param name="current"></param>
        void ReconstructPath(Dictionary<Path_Node<T>, Path_Node<T>> Came_From, Path_Node<T> current)
        {
            //Debug.Log("Reconstruct path.");
            //path = new PathStack();
            path = new List<T>();
            path.Add(current.data); // the final step in the path is the starting current node

            // while the map Came_From contians the current node, the current node is not the start node
            // find the next node in the path and add it to total_path
            while (Came_From.ContainsKey(current))
            {
                current = Came_From[current];
                path.Add(current.data);
            }
        }

    }
}

public class PathNotFoundException : Exception
    {
        public PathNotFoundException() { }
        public PathNotFoundException(string message) : base(message) { }
        public PathNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

