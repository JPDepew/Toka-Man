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

    // Start is called before the first frame update
    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        tilemapSize = tilemap.size.x * tilemap.size.y;
        player = FindObjectOfType<PlayerController>().gameObject;
        gameMaster = GameMaster.instance;
        grid = GridContainer.instance;

        StartCoroutine(Move());
        FindPath(grid.GetNodeFromWorldPoint(), grid.GetNodeFromWorldPoint2());
    }

    // Update is called once per frame
    void Update()
    {
        FindPath(grid.GetNodeFromWorldPoint(), grid.GetNodeFromWorldPoint2());
    }

    void FindPath(Node startPos, Node targetPos)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        startPos.cur = true;
        openSet.Add(startPos);

        targetPos.target = true;
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
        path.Reverse();

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
            yield return MoveToPos(GetNextPath());
        }
    }

    private IEnumerator MoveToPos(Vector3? newPos)
    {
        Vector3 convertedNewPos = Vector3.zero;
        if (newPos != null)
        {
            convertedNewPos = (Vector3)newPos;
        }
        Vector3 direction = (convertedNewPos - transform.position).normalized;

        while (!ReachedDestination(transform.position, convertedNewPos, direction))
        {
            transform.position = transform.position + direction * speed * Time.deltaTime;
            yield return null;
        }
        transform.position = convertedNewPos;
    }

    private Vector3Int? GetNextPath()
    {
        float playerXDiff = player.transform.position.x - transform.position.x;
        float playerYDiff = player.transform.position.y - transform.position.y;
        float dirX = Mathf.Sign(playerXDiff);
        float dirY = Mathf.Sign(playerYDiff);
        int gridOffset = -1;

        if (Mathf.Abs(playerXDiff) > Mathf.Abs(playerYDiff))
        {
            if (tilemap.GetTile(new Vector3Int((int)(transform.position.x + dirX) + gridOffset, (int)transform.position.y + gridOffset, 0)) == null)
            {
                return new Vector3Int((int)(transform.position.x + dirX), (int)transform.position.y, 0);
            }
            else if (tilemap.GetTile(new Vector3Int((int)transform.position.x + gridOffset, (int)(transform.position.y + dirY) + gridOffset, 0)) == null)
            {
                return new Vector3Int((int)transform.position.x, (int)(transform.position.y + dirY), 0);
            }
            else
            {
                if (tilemap.GetTile(new Vector3Int((int)(transform.position.x - dirX) + gridOffset, (int)transform.position.y + gridOffset, 0)) == null)
                {
                    return new Vector3Int((int)(transform.position.x - dirX), (int)transform.position.y, 0);
                }
                else
                {
                    return new Vector3Int((int)transform.position.x, (int)(transform.position.y - dirY), 0);
                }
            }
        }
        else
        {
            if (tilemap.GetTile(new Vector3Int((int)transform.position.x + gridOffset, (int)(transform.position.y + dirY) + gridOffset, 0)) == null)
            {
                return new Vector3Int((int)transform.position.x, (int)(transform.position.y + dirY), 0);
            }
            else if (tilemap.GetTile(new Vector3Int((int)(transform.position.x + dirX) + gridOffset, (int)transform.position.y + gridOffset, 0)) == null)
            {
                return new Vector3Int((int)(transform.position.x + dirX), (int)transform.position.y, 0);
            }
            else
            {
                if (tilemap.GetTile(new Vector3Int((int)transform.position.x + gridOffset, (int)(transform.position.y - dirY) + gridOffset, 0)) == null)
                {
                    return new Vector3Int((int)transform.position.x, (int)(transform.position.y - dirY), 0);
                }
                else
                {
                    return new Vector3Int((int)(transform.position.x + dirX), (int)transform.position.y, 0);
                }
            }
        }
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
