using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public List<Square> Squares;
    public Square GetSquareAt(int row, int column)
    {
        return Squares[row * width + column];
    }
}
