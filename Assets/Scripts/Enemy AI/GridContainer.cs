using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridContainer : MonoBehaviour
{
    public Node[,] grid;
    public static GridContainer instance;

    private void Awake()
    {
        instance = this;
    }

    public Node GetNodeFromWorldPoint(Vector3 position)
    {
        //float percentX = position.x / grid.GetLength(0);
        //float percentY = position.y / grid.GetLength(1);
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt(())

        return grid[(int)position.x, (int)position.y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        List<Vector2> posToCheck = new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        for (int i = 0; i < posToCheck.Count; i++)
        {
            int checkX = node.gridX + (int)posToCheck[i].x;
            int checkY = node.gridY + (int)posToCheck[i].y;

            if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }

        return neighbors;
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Gizmos.color = grid[i, j].walkable ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
                    if (path != null)
                    {
                        if (path.Contains(grid[i, j]))
                        {
                            Gizmos.color = Color.black;
                        }
                    }

                    if(grid[i, j].cur)
                    {
                        Gizmos.color = Color.green;
                    }
                    if (grid[i, j].target)
                    {
                        Gizmos.color = Color.blue;
                    }
                    Gizmos.DrawCube(grid[i, j].position, Vector3.one);
                }
            }
        }
    }
}
