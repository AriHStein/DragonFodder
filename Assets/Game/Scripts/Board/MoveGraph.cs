using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveGraph : IPath_Graph<BoardSquare>
{

    public Dictionary<BoardSquare, Path_Node<BoardSquare>> Graph { get; protected set; }

    public MoveGraph(Dictionary<BoardSquare, Path_Node<BoardSquare>> graph)
    {
        Graph = graph;
    }
    //public MoveGraph(BoardSquare[,] squares, bool unitsBlockMovement = true)
    //{
    //    if(squares == null || squares.GetLength(0) == 0 || squares.GetLength(1) == 0)
    //    {
    //        Debug.LogError("Invalid board.");
    //        return;
    //    }

    //    Graph = new Dictionary<BoardSquare, Path_Node<BoardSquare>>();

    //    for(int x = 0; x < squares.GetLength(0); x++)
    //    {
    //        for(int y = 0; y < squares.GetLength(1); y++)
    //        {
    //            Path_Node<BoardSquare> node = new Path_Node<BoardSquare>(squares[x, y]);
    //            Graph.Add(squares[x, y], node);
    //        }
    //    }

    //    for (int x = 0; x < squares.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < squares.GetLength(1); y++)
    //        {
    //            Path_Node<BoardSquare> node = Graph[squares[x, y]];
    //            List<Path_Edge<BoardSquare>> edges = new List<Path_Edge<BoardSquare>>();

    //            List<Vector2Int> neighbors = new List<Vector2Int> {
    //                new Vector2Int(x - 1, y),
    //                new Vector2Int(x, y - 1),
    //                new Vector2Int(x + 1, y),
    //                new Vector2Int(x, y + 1)
    //            };

    //            foreach(Vector2Int neighbor in neighbors)
    //            {
    //                if (neighbor.x >= 0 && neighbor.y >= 0 && 
    //                    neighbor.x < squares.GetLength(0) && neighbor.y < squares.GetLength(1) &&
    //                    (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
    //                {
    //                    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
    //                }
    //            }
                

    //            //neighbor = new Vector2Int(x, y - 1);
    //            //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
    //            //{
    //            //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
    //            //}

    //            //neighbor = new Vector2Int(x + 1, y);
    //            //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
    //            //{
    //            //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
    //            //}

    //            //neighbor = new Vector2Int(x, y + 1);
    //            //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
    //            //{
    //            //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
    //            //}

    //            node.edges = edges;
    //        }
    //    }
    //}
}
