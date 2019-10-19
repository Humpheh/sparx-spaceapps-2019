using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    public GameObject canvas;
    public GameObject cityText;
    public GameObject cityIcon;
    public GameObject doctorIcon;
    private MapTile[][] map;
    private bool[][] landMap;

    // Bounds of the map
    private static int mapWidth = 82;
    private static int mapHeight = 41;

    // Size of the grid on the map
    public static int gridWidth = 360; //mapWidth*2;
    public static int gridHeight = 180; //mapHeight*2;

    private List<CityControl> cities = new List<CityControl>();

    // Start is called before the first frame update
    void Start()
    {
        _mapSingleton = this;
        mozData = gameObject.AddComponent<MozDataParse>();

        BuildLandData();

        CreateMap();
        //        CreateMapOverlay();
        CreateMapCities();

        Modal.OpenModal(
            "You are the Mosquito Defender!",
            "<b>The world needs your help!</b>\\nThe world health organisation is gone, and you're the only one that can save the world from mosquitoes.",
            Started
        );
    }

    public ResourceManager GetResourceManager()
    {
        return GetComponent<ResourceManager>();
    }

    void Started(string option)
    {
        Popup.SpawnPanel(Vector3.zero, "https://data.globe.gov/system/photos/2019/05/23/1079041/original.jpg");
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
        GameObject iconPrefab;
        if (location.isStatic) iconPrefab = cityIcon;
        else iconPrefab = doctorIcon;

        // Circle at the location (is clickable)
        Vector3 worldLocation;
        if (location.usingLatLon) worldLocation = new Vector3(GridToMapX((int)location.latlon.x), GridToMapY((int)location.latlon.y), -1);
        else worldLocation = new Vector3(location.position.x, location.position.y, -1);

        var icon = Instantiate(iconPrefab);
        var city = icon.GetComponent<CityControl>();
        city.location = location;
        city.worldLocation = worldLocation;
        if (location.isLocked) icon.GetComponent<Image>().color = Color.grey;
        icon.transform.SetParent(canvas.transform, false);

        // Text overlay for the location
        var text = Instantiate(cityText);
        text.GetComponent<Text>().text = location.city;
        text.transform.SetParent(canvas.transform, false);
        city.text = text;
        city.SetPosition();
        city.UpdateText();

        location.obj = icon;
        cities.Add(city);
    }

    [CanBeNull]
    public CityControl FindCloseCity(Vector3 worldLocation, float threshold = 1)
    {
        foreach (var city in cities)
        {
            float num1 = worldLocation.x - city.worldLocation.x;
            float num2 = worldLocation.y - city.worldLocation.y;
            var diff = Math.Sqrt((double)num1 * num1 + (double)num2 * num2);
            if (diff < threshold)
            {
                if (city.location.isLocked) return null;
                else return city;
            }
        }

        return null;
    }

    public void DropDoctor(Vector3 worldLocation)
    {
        var closeCity = FindCloseCity(worldLocation);
        if (closeCity != null)
        {
            closeCity.AddDoctor();
        }
        else
        {
            // None were nearby, create a new location
            var location = new Location(worldLocation, 1);
            CreateLocation(location);
        }
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
            //Debug.Log(hit.point);
            //Debug.Log(MapPointToGrid(hit.point));
            var location = CityControl.CurrentSelection;
            if (location != null && location.CanRemoveDoctor())
            {
                Vector3 toLocation = hit.point;
                var closeCity = FindCloseCity(hit.point);
                if (closeCity != null)
                {
                    toLocation = closeCity.worldLocation;
                }

                location.RemoveDoctor();
                PlaneBehaviour.SpawnPlane(location.worldLocation, toLocation);
                location.TryRemoveCity();
                location.Deselect();
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
