using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoardSquareContext : IAbilityContext
{
    public int Value { get; protected set; }
    public Unit Actor { get; protected set; }
    public BoardSquare Square { get; protected set; }
    public Board Board { get; protected set; }

    public SingleBoardSquareContext(int value, Unit actor, BoardSquare square, Board board)
    {
        Value = value;
        Actor = actor;
        Square = square;
        Board = board;
    }
}
