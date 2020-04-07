using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 5;
    private Tilemap tilemap;
    private float tilemapSize;
    private GameObject player;
    GridContainer grid;
    GameMaster gameMaster;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        tilemapSize = tilemap.size.x * tilemap.size.y;
        player = FindObjectOfType<PlayerController>().gameObject;
        gameMaster = GameMaster.instance;
        grid = GridContainer.instance;

        StartCoroutine(Move());
    }

    /// <summary>
    /// A* algorithm
    /// </summary>
    /// <param name="startPos">Node at which to start the pathat</param>
    /// <param name="targetPos">Target node</param>
    void FindPath(Node startPos, Node targetPos)
    {
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startPos);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // Path found
            if (currentNode == targetPos)
            {
                RetracePath(startPos, targetPos);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetPos);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the path in a linked list
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node curNode = endNode;

        while (curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        //path.Reverse();

        grid.path = path;
    }

    /// <summary>
    /// Gets the distance between two nodes. This is simply the Xdst + Ydst, since everything is on a grid.
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    int GetDistance(Node nodeA, Node nodeB)
    {
        int xDst = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int yDst = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return xDst + yDst;
    }

    /// <summary>
    /// Gets the next node to move from from the path.
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator Move()
    {
        while (true)
        {
            FindPath(grid.GetNodeFromWorldPoint(player.transform.position), grid.GetNodeFromWorldPoint(transform.position));
            if (grid.path.Count > 0)
                grid.path.RemoveAt(0);
            if (grid.path.Count > 0)
            {
                Node nextPos = grid.path[0];
                grid.path.RemoveAt(0);
                yield return MoveToPos(nextPos.position);
            }
            else
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// Moves to the new position.
    /// </summary>
    /// <param name="newPos"></param>
    /// <returns>null (once a frame)</returns>
    private IEnumerator MoveToPos(Vector3 newPos)
    {
        Vector3 direction = (newPos - transform.position).normalized;

        while (!ReachedDestination(transform.position, newPos, direction))
        {
            transform.position = transform.position + direction * speed * Time.deltaTime;
            yield return null;
        }
        transform.position = newPos;
    }

    /// <summary>
    /// Checks if this has reached the destination by seeing if the start position is overlapping with the end position.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="destination"></param>
    /// <param name="direction"></param>
    /// <returns>Whether or not the destination has been reached</returns>
    private bool ReachedDestination(Vector3 start, Vector3 destination, Vector3 direction)
    {
        if (direction == Vector3.right)
        {
            return start.x >= destination.x;
        }
        else if (direction == Vector3.left)
        {
            return start.x <= destination.x;
        }
        else if (direction == Vector3.up)
        {
            return start.y >= destination.y;
        }
        else if (direction == Vector3.down)
        {
            return start.y <= destination.y;
        }
        else
        {
            return true;
        }
    }
}
