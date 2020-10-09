using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BoardGraph
{

    public MoveGraph Walking { get; private set; }
    public MoveGraph Flying { get; private set; }

    public BoardGraph(BoardSquare[,] squares)
    {
        if (squares == null || squares.GetLength(0) == 0 || squares.GetLength(1) == 0)
        {
            Debug.LogError("Invalid board.");
            return;
        }

        Dictionary<BoardSquare, Path_Node<BoardSquare>>  walking = new Dictionary<BoardSquare, Path_Node<BoardSquare>>();
        Dictionary < BoardSquare, Path_Node < BoardSquare >> flying = new Dictionary<BoardSquare, Path_Node<BoardSquare>>();

        for (int x = 0; x < squares.GetLength(0); x++)
        {
            for (int y = 0; y < squares.GetLength(1); y++)
            {
                if(squares[x,y] == null)
                {
                    Debug.Log($"Square at ({x},{y}) is null. wtf!");
                }
                
                Path_Node<BoardSquare> walkNode = new Path_Node<BoardSquare>(squares[x, y]);
                Path_Node<BoardSquare> flyNode = new Path_Node<BoardSquare>(squares[x, y]);

                walking.Add(squares[x, y], walkNode);
                flying.Add(squares[x, y], flyNode);

            }
        }

        for (int x = 0; x < squares.GetLength(0); x++)
        {
            for (int y = 0; y < squares.GetLength(1); y++)
            {
                //Path_Node<BoardSquare> node = Graph[squares[x, y]];
                List<Path_Edge<BoardSquare>> walkEdges = new List<Path_Edge<BoardSquare>>();
                List<Path_Edge<BoardSquare>> flyEdges = new List<Path_Edge<BoardSquare>>();

                List<Vector2Int> neighbors = new List<Vector2Int> {
                    new Vector2Int(x - 1, y),
                    new Vector2Int(x, y - 1),
                    new Vector2Int(x + 1, y),
                    new Vector2Int(x, y + 1)
                };

                foreach (Vector2Int neighbor in neighbors)
                {
                    if (neighbor.x >= 0 && neighbor.y >= 0 &&
                        neighbor.x < squares.GetLength(0) && neighbor.y < squares.GetLength(1))
                    {
                        flyEdges.Add(new Path_Edge<BoardSquare>(flying[squares[neighbor.x, neighbor.y]], 1));
                        if(squares[neighbor.x, neighbor.y].Unit == null)
                        {
                            walkEdges.Add(new Path_Edge<BoardSquare>(walking[squares[neighbor.x, neighbor.y]], 1));
                        }
                    }
                }


                //neighbor = new Vector2Int(x, y - 1);
                //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
                //{
                //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
                //}

                //neighbor = new Vector2Int(x + 1, y);
                //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
                //{
                //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
                //}

                //neighbor = new Vector2Int(x, y + 1);
                //if (neighbor.x >= 0 && (!unitsBlockMovement || squares[neighbor.x, neighbor.y].Unit != null))
                //{
                //    edges.Add(new Path_Edge<BoardSquare>(Graph[squares[neighbor.x, neighbor.y]], 1));
                //}

                walking[squares[x,y]].edges = walkEdges;
                flying[squares[x, y]].edges = flyEdges;

            }
        }

        Walking = new MoveGraph(walking);
        Flying = new MoveGraph(flying);
    }
}
