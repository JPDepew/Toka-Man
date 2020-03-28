using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool walkable;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get {
            return gCost + hCost;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = walkable ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
