using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject prefab;
    
    private MapTile[][] map;
    
    // Bounds of the map
    private static int mapWidth = 82;
    private static int mapHeight = 41;

    // Size of the grid on the map
    public static int gridWidth = 360; //mapWidth*2;
    public static int gridHeight = 180; //mapHeight*2;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
//        CreateMapOverlay();
    }

    private void CreateMap()
    {
        // Create the multidimensional array of map tiles
        map = new MapTile[gridHeight][];
        for (var y = 0; y < gridHeight; y++)
        {
            map[y] = new MapTile[gridWidth];
            for (var x = 0; x < gridWidth; x++)
            {
                var realX = (float)mapWidth / gridWidth * (x - (float) gridWidth / 2);
                var realY = (float)mapHeight / gridHeight * (y - (float) gridHeight / 2);
                map[y][x] = new MapTile(x, y, new Vector2(realX, realY), true);//Random.value < 0.5);
            }
        }
    }

    private void CreateMapOverlay()
    {
        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                var tile = map[y][x];
                if (tile.on)
                {
                    var point = Instantiate(prefab, new Vector3(tile.pos.x, tile.pos.y, -1), Quaternion.Euler(-90, 0, 0));
                    point.transform.parent = transform;
                    point.GetComponent<MeshRenderer>().material.color = new Color(tile.testValue, 0, 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AdvanceMapTime()
    {
        
    }
}

class MapTile
{
    public int x, y;
    public Vector2 pos;
    public bool on;
    public float testValue;
    
    public MapTile(int x, int y, Vector2 pos, bool on)
    {
        this.x = x;
        this.y = y;
        this.on = on;
        this.pos = pos;
        this.testValue = Random.value;
    }
}