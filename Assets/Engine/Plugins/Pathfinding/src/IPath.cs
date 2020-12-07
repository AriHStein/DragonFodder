using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public interface IPath<T>
    {
        T Dequeue();
        T Peek();
        T End();
        T Start();
        int Length();

        T this[int index] { get; }
    }
}
