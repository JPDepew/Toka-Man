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

    void FindPath(Node startPos, Node targetPos)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startPos);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

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

    int GetDistance(Node nodeA, Node nodeB)
    {
        int xDst = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int yDst = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return xDst + yDst;
    }

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
