using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    private static Map _mapSingleton;
    public static Map GetSingleton()
    {
        return _mapSingleton;
    }

    private bool _paused = false;
    
    public MozDataParse mozData;
    private Locations locations = new Locations();

    public GameObject prefab;
    public GameObject cityPrefab;
    public GameObject canvas;
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
        _mapSingleton = this;
        mozData = gameObject.AddComponent<MozDataParse>();

        BuildLandData();

        CreateMap();
        CreateMapOverlay();
        CreateMapCities();

//        Modal.OpenModal(
//            "You are the Mosquito Defender!", 
//            "<b>The world needs your help!</b>\\nThe world health organisation is gone, and you're the only one that can save the world from mosquitoes.",
//            Started
//        );

        Choice.OpenChoice("What do you want to do?", "Choose something to do please", new []
        {
            new ChoiceOption("Fly 1 there", "$1000", delegate {  }),
            new ChoiceOption("Fly 2 there", "$2000", delegate {  }),
            new ChoiceOption("Fly 3 there", "$2500", delegate {  }),
        }, delegate { Started(""); });
    }

    void Started(string option)
    {
        Debug.Log("Start");;
    }

    public void PauseMap()
    {
        _paused = true;
    }

    public void UnpauseMap()
    {
        _paused = false;
    }

    public bool IsPaused()
    {
        return _paused;
    }
    
    void BuildLandData()
    {
        TextAsset bindata = UnityEngine.Resources.Load("landData") as TextAsset;
        var lines = bindata.text.Split('\n');

        var y = 0;
        landMap = new bool[gridHeight][];
        foreach (var line in lines)
        {
            if (y >= gridHeight) break;
            var x = 0;
            landMap[gridHeight - 1 - y] = new bool[gridWidth];
            foreach (var c in line)
            {
                landMap[gridHeight - 1 - y][x] = c == '1';
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
                if (mozData.HasEvent(x, y))
                {
                    var point = Instantiate(prefab, new Vector3(tile.pos.x, tile.pos.y, -1), Quaternion.Euler(-90, 0, 0));
                    point.transform.parent = transform;
                    point.GetComponent<MeshRenderer>().material.color = new Color(y / (float)map.Length, x / (float)map[y].Length, 0);
                }
            }
        }
    }

    private void CreateMapCities()
    {
        foreach (var location in locations.LocationsList)
        {
            CreateLocation(location);
        }
    }

    public void CreateLocation(Location location)
    {
        GameObject cityText = UnityEngine.Resources.Load("CityName") as GameObject;

        GameObject iconPrefab;
        if (location.isStatic) iconPrefab = UnityEngine.Resources.Load("CityIcon") as GameObject;
        else iconPrefab = UnityEngine.Resources.Load("DoctorIcon") as GameObject;

        // Circle at the location (is clickable)
        var worldLocation = new Vector3(GridToMapX(location.x), GridToMapY(location.y), -1);
        var icon = Instantiate(iconPrefab);
        icon.GetComponent<CityControl>().location = location;
        icon.GetComponent<CityControl>().worldLocation = worldLocation;
        icon.transform.SetParent(canvas.transform, false);

        // Text overlay for the location
        var text = Instantiate(cityText);
        text.GetComponent<Text>().text = location.city;
        text.transform.SetParent(canvas.transform, false);
        icon.GetComponent<CityControl>().text = text;

        location.obj = icon;
    }

    public void DropDoctor(Vector3 worldLocation)
    {
        var lat = MapXToGrid(worldLocation.x);
        var lon = MapYToGrid(worldLocation.y);    
        
    }

    public float GridToMapX(int GridX)
    {
        return (float)mapWidth / gridWidth * (GridX + 0.5f - (float)gridWidth / 2);
    }

    public float GridToMapY(int GridY)
    {
        return (float)mapHeight / gridHeight * (GridY + 0.5f - (float)gridHeight / 2);
    }

    public int MapXToGrid(float mapX)
    {
        return (int)Math.Round((mapX - 0.5f + ((float)mapWidth / 2)) / mapWidth * gridWidth);
    }

    public int MapYToGrid(float mapY)
    {
        return (int)Math.Round((mapY - 0.5f + ((float)mapHeight / 2)) / mapHeight * gridHeight);
    }

    public (int, int) MapPointToGrid(Vector2 point)
    {
        return (MapXToGrid(point.x), MapYToGrid(point.y));
    }

    void Update()
    {
        if (IsPaused()) return;
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            Debug.Log(hit.point);
            Debug.Log(MapPointToGrid(hit.point));
            if (CityControl.CurrentSelection != null)
            {
                PlaneBehaviour.SpawnPlane(CityControl.CurrentSelection.worldLocation, hit.point);
                CityControl.CurrentSelection.Deselect();
            }
        }
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
