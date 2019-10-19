using UnityEngine;

public class Map : MonoBehaviour
{
    private MozDataParse mozData;
    private Locations locations = new Locations();
    
    public GameObject prefab;
    public GameObject cityPrefab;
    
    private MapTile[][] map;
    private bool[][] landMap;
    
    // Bounds of the map
    private static int mapWidth = 82;
    private static int mapHeight = 41;

    // Size of the grid on the map
    public static int gridWidth = 360; //mapWidth*2;
    public static int gridHeight = 180; //mapHeight*2;

    // Start is called before the first frame update
    void Start()
    {
        mozData = gameObject.AddComponent<MozDataParse>();
        
        BuildLandData();
        
        CreateMap();
        CreateMapOverlay();
        CreateMapCities();

    }

    void BuildLandData()
    {
        TextAsset bindata = Resources.Load("landData") as TextAsset;
        var lines = bindata.text.Split('\n');

        var y = 0;
        landMap = new bool[gridHeight][];
        foreach (var line in lines)
        {
            if (y >= gridHeight) break;
            var x = 0;
            landMap[gridHeight-1-y] = new bool[gridWidth];
            foreach (var c in line)
            {
                landMap[gridHeight-1-y][x] = c == '1';
                x++;
            }
            y++;
        }
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
                var realX = GridToMapX(x);
                var realY = GridToMapY(y);
                map[y][x] = new MapTile(x, y, new Vector2(realX, realY), landMap[y][x]);
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
                if (tile.isLand)
                {
                    var point = Instantiate(prefab, new Vector3(tile.pos.x, tile.pos.y, -1), Quaternion.Euler(-90, 0, 0));
                    point.transform.parent = transform;
                    point.GetComponent<MeshRenderer>().material.color = new Color((float)y/(float)map.Length, (float)x/(float)map[y].Length, 0);
                }
            }
        }
    }

    private void CreateMapCities()
    {
        for (var l = 0; l < locations.LocationsList.Length; l++)
        {
            var point = Instantiate(cityPrefab, new Vector3(GridToMapX(locations.LocationsList[l].x), GridToMapY(locations.LocationsList[l].y), -1), Quaternion.Euler(-90, 0, 0));
            point.transform.parent = transform;
            point.GetComponent<MeshRenderer>().material.color = new Color(1,1,0);
        }
    }

    private float GridToMapX(int GridX)
    {
        float x = (float)mapWidth / gridWidth * (GridX+0.5f - (float)gridWidth / 2);
        return x;
    }
    private float GridToMapY(int GridY)
    {
        float y = (float)mapHeight / gridHeight * (GridY+0.5f - (float)gridHeight / 2);
        return y;
    }
}

class MapTile
{
    public int x, y;
    public Vector2 pos;
    public bool isLand;
    public float testValue;
    
    public MapTile(int x, int y, Vector2 pos, bool isLand)
    {
        this.x = x;
        this.y = y;
        this.isLand = isLand;
        this.pos = pos;
        this.testValue = Random.value;
    }
}