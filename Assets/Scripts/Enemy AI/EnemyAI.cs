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

    // Start is called before the first frame update
    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        tilemapSize = tilemap.size.x * tilemap.size.y;
        player = FindObjectOfType<PlayerController>().gameObject;

        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(player.transform.position);
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        List<Vector3> openSet = new List<Vector3>();
        HashSet<Vector3> closedSet = new HashSet<Vector3>();

        openSet.Add(startPos);

        while(openSet.Count > 0)
        {

        }
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
