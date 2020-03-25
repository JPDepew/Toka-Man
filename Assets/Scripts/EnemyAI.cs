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
        Debug.Log("====================");
        Debug.Log(newPos);
        Debug.Log(transform.position);
        Vector3 direction = (convertedNewPos - transform.position).normalized;

        while (!AlmostEqual(transform.position, convertedNewPos, 0.05f))
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

        //Debug.Log("Right: " + tilemap.GetTile(new Vector3Int((int)(transform.position.x + 1) - 1, (int)transform.position.y - 1, 0)));
        //Debug.Log("Left: " + tilemap.GetTile(new Vector3Int((int)(transform.position.x - 1) - 1, (int)transform.position.y - 1, 0)));
        //Debug.Log("Up: " + tilemap.GetTile(new Vector3Int((int)(transform.position.x) - 1, (int)transform.position.y + 1 - 1, 0)));
        //Debug.Log("Down: " + tilemap.GetTile(new Vector3Int((int)(transform.position.x) - 1, (int)transform.position.y - 1 - 1, 0)));

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
                return null;
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
                return null;
            }
        }
    }

    private bool AlmostEqual(Vector3 pos1, Vector3 pos2, float leeway)
    {
        return pos1.x > pos2.x - leeway && pos1.x < pos2.x + leeway && pos1.y > pos2.y - leeway && pos1.y < pos2.y + leeway;
    }
}
