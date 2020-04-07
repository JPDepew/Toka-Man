using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public GameObject dataPoint;
    public Transform dataPointsParent;
    public Tilemap tilemap;
    public Transform gridContainer;

    GridContainer gridRef;

    public Node[,] grid;

    private int dataPointCount = 0;
    private int playerDataPointCount = 0;
    private Transform player;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>().transform;
        gridRef = GetComponent<GridContainer>();
        InstantiateDataPoints();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.onDataPickup += IncrementPlayerPickupCount;
    }

    /// <summary>
    /// Instantiates datapoints on all walkable areas on the grid.
    /// Initializes the grid to the size of the tilemap and adds a node for each position marked as walkable/non-walkable.
    /// </summary>
    void InstantiateDataPoints()
    {
        int tilemapSize = tilemap.size.x * tilemap.size.y;
        Vector3Int originalPos = new Vector3Int(tilemap.origin.x, tilemap.origin.y, 0);
        Vector3Int pos = originalPos;
        Vector3Int offset = new Vector3Int((int)tilemap.transform.position.x, (int)tilemap.transform.position.y, 0);

        grid = new Node[tilemap.size.x, tilemap.size.y];

        for (int x = 0; x < tilemap.size.x; x++)
        {
            for (int y = 0; y < tilemap.size.y; y++)
            {
                if (tilemap.GetTile(pos) == null)
                {
                    if (!AlmostEqual(new Vector2(pos.x + 1, pos.y + 1), player.position, 0.01f))
                    {
                        InstantiateDataPointSection(pos + offset);
                        dataPointCount++;
                    }
                    grid[x, y] = new Node(true, new Vector3(pos.x + 1, pos.y + 1) + offset, x, y);
                }
                else
                {
                    grid[x, y] = new Node(false, new Vector3(pos.x + 1, pos.y + 1) + offset, x, y);
                }

                pos = new Vector3Int(pos.x, (pos.y + 1), 0);
            }
            pos = new Vector3Int((pos.x + 1), originalPos.y, 0);
        }
        gridRef.grid = grid;
    }

    void InstantiateDataPointSection(Vector3Int pos)
    {
        Instantiate(dataPoint, new Vector3(pos.x + 1, pos.y + 1), Quaternion.Euler(Vector3.up), dataPointsParent);
    }

    void IncrementPlayerPickupCount()
    {
        playerDataPointCount++;
        // check if >=
    }

    /// <summary>
    /// Checks if two Vector3 vars are almost equal.
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="leeway">The float value leeway for comparing two positions.</param>
    /// <returns></returns>
    private bool AlmostEqual(Vector3 pos1, Vector3 pos2, float leeway)
    {
        return pos1.x > pos2.x - leeway && pos1.x < pos2.x + leeway && pos1.y > pos2.y - leeway && pos1.y < pos2.y + leeway;
    }
}
