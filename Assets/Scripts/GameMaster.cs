using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMaster : MonoBehaviour
{
    public GameObject dataPoint;
    public Transform dataPointsParent;
    public Tilemap tilemap;
    public Transform gridContainer;
    public GameObject node;

    Node[,] grid;

    private int dataPointCount = 0;
    private int playerDataPointCount = 0;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.onDataPickup += IncrementPlayerPickupCount;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame()
    {
        player = FindObjectOfType<PlayerController>().transform;
        InstantiateDataPoints();
    }

    void InstantiateDataPoints()
    {
        int tilemapSize = tilemap.size.x * tilemap.size.y;
        Vector3Int originalPos = new Vector3Int(tilemap.origin.x, tilemap.origin.y, 0);
        Vector3Int pos = originalPos;

        grid = new Node[tilemap.size.y, tilemap.size.x];

        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j < tilemap.size.y; j++)
            {
                if (tilemap.GetTile(pos) == null)
                {
                    if (!AlmostEqual(new Vector2(pos.x + 1, pos.y + 1), player.position, 0.01f))
                    {
                        InstantiateDataPointSection(pos);
                        dataPointCount++;
                    }
                    GameObject tempNode = Instantiate(node, new Vector3(pos.x + 1, pos.y + 1), transform.rotation, gridContainer);
                    grid[j, i] = tempNode.GetComponent<Node>();
                    grid[j, i].walkable = true;
                }
                else
                {
                    Debug.Log(new Vector2(pos.x + 1, pos.y + 1));
                    GameObject tempNode = Instantiate(node, new Vector3(pos.x + 1, pos.y + 1), transform.rotation, gridContainer);
                    grid[j, i] = tempNode.GetComponent<Node>();
                    grid[j, i].walkable = false;
                }

                pos = new Vector3Int(pos.x, (pos.y + 1), 0);
            }
            pos = new Vector3Int((pos.x + 1), originalPos.y, 0);
        }
    }

    void InstantiateDataPointSection(Vector3Int pos)
    {
        Instantiate(dataPoint, new Vector3(pos.x + 1, pos.y + 1), Quaternion.Euler(Vector3.up), dataPointsParent);
        //InstantiateRightDataPoints(pos);
        //InstantiateUpDataPoints(pos);
    }

    void InstantiateRightDataPoints(Vector3Int pos)
    {
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, pos.z);
        if (tilemap.GetTile(rightPos) == null)
        {
            // Create more data points to the right
            dataPointCount++;
        }
    }

    void InstantiateUpDataPoints(Vector3Int pos)
    {
        Vector3Int upPos = new Vector3Int(pos.x, pos.y + 1, pos.z);
        if (tilemap.GetTile(upPos) == null)
        {
            // Create more data points in the upwards direction
            dataPointCount++;
        }
    }

    void IncrementPlayerPickupCount()
    {
        playerDataPointCount++;
        // check if >=
    }

    private bool AlmostEqual(Vector3 pos1, Vector3 pos2, float leeway)
    {
        return pos1.x > pos2.x - leeway && pos1.x < pos2.x + leeway && pos1.y > pos2.y - leeway && pos1.y < pos2.y + leeway;
    }
}
