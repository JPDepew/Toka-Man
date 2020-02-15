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
        //Debug.Log(pos);
        //Debug.Log(tilemap.cellSize);
        //Debug.Log(tilemap.size);
        TileBase[] allTiles = tilemap.GetTilesBlock(tilemap.cellBounds);

        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j < tilemap.size.y; j++)
            {
                if (tilemap.GetTile(pos) == null)
                {
                    Instantiate(dataPoint, new Vector3(pos.x + 1, pos.y + 1), Quaternion.Euler(Vector3.up), dataPointsParent);
                    //Debug.Log(tilemap.Get(pos).name + " Not null " + pos);
                }
                //else
                //{
                //    //Debug.Log(tilemap.GetSprite(pos) + " " + pos);
                //}

                pos = new Vector3Int(pos.x, (int)(pos.y + 1), 0);
            }
            pos = new Vector3Int((int)(pos.x + 1), originalPos.y, 0);
        }
    }
}
