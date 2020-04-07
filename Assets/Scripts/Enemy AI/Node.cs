using System.Collections;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 position;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    public bool cur;
    public bool target;
    int heapIndex;

    public int fCost
    {
        get {
            return gCost + hCost;
        }
    }

    public Node(bool _walkable, Vector3 _position, int _gridX, int _gridY)
    {
        walkable = _walkable;
        position = _position;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /// <summary>
    /// Implementing IComparable.
    /// Compare fCost. If fCosts are equal, compare hCost.
    /// Return -compare because lower integer values have higher priority.
    /// </summary>
    /// <param name="nodeToCompare"></param>
    /// <returns>Interger value (1, 0, -1)</returns>
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
