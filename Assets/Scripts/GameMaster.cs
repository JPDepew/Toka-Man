using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMaster : MonoBehaviour
{
    public GameObject dataPoint;
    public Transform dataPointsParent;
    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame()
    {
        InstantiateDataPoints();
    }

    void InstantiateDataPoints()
    {
        int tilemapSize = tilemap.size.x * tilemap.size.y;
        Vector3Int originalPos = new Vector3Int(tilemap.origin.x, tilemap.origin.y, 0);
        Vector3Int pos = originalPos;

        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j < tilemap.size.y; j++)
            {
                if (tilemap.GetTile(pos) == null)
                {
                    InstantiateDataPointSection(pos);
                }

                pos = new Vector3Int(pos.x, (int)(pos.y + 1), 0);
            }
            pos = new Vector3Int((int)(pos.x + 1), originalPos.y, 0);
        }
    }

    void InstantiateDataPointSection(Vector3Int pos)
    {
        Instantiate(dataPoint, new Vector3(pos.x + 1, pos.y + 1), Quaternion.Euler(Vector3.up), dataPointsParent);
        InstantiateRightDataPoints(pos);
        InstantiateUpDataPoints(pos);
    }

    void InstantiateRightDataPoints(Vector3Int pos)
    {
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, pos.z);
        if (tilemap.GetTile(rightPos) == null)
        {
            // Create more data points to the right
        }
    }

    void InstantiateUpDataPoints(Vector3Int pos)
    {
        Vector3Int upPos = new Vector3Int(pos.x, pos.y + 1, pos.z);
        if (tilemap.GetTile(upPos) == null)
        {
            // Create more data points in the upwards direction
        }
    }
}
