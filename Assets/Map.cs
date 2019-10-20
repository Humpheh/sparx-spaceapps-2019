using System;
using System.Collections;
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

    public PlaneBehaviour.PlaneType dispatchType;

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
            "You're in charge of dispatching doctors as a first line of defence against mosquito outbreaks.\\n " +
            "Use the data sent in by the public to help inform your decisions.\\n\\n<b>Good luck!</b>",
            Started
        );
    }

    public ResourceManager GetResourceManager()
    {
        return GetComponent<ResourceManager>();
    }

    void Started(string option)
    {
        Modal.OpenModal(
            "Instructions",
            "Use the mouse to navigate around the map and click on cities to choose actions.\\n" +
            "Keep an eye out for NASA GLOBE citizen science reports of mosquito sightings."
        );
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
        city.effectiveness = location.effectiveness;
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
                return city;
            }
        }

        return null;
    }

    public float NearbyDoctorTimeMultipler(Vector3 loc, float threshold = 10)
    {
        const float growSpeed = 1f;

        float totalDoctors = 0; // effectiveness
        float nearbyDoctors = 0;
        foreach (var city in cities)
        {
            var distance = Vector2.Distance(
                new Vector2(loc.x, loc.y),
                new Vector2(city.worldLocation.x, city.worldLocation.y)
            );

            totalDoctors += city.location.effectiveness;
            if (distance < 4)
            {
                nearbyDoctors += city.location.effectiveness;
            }
        }

        return growSpeed - totalDoctors / 100f - (nearbyDoctors * growSpeed * 0.9f);
    }

    public void AddRemoveRandomDoc(int numberToAddOrRemove)
    {
        foreach (var city in cities)
        {
            // Only affect cities that aren't locked
            if (!city.location.isLocked && numberToAddOrRemove != 0)
            {
                while (numberToAddOrRemove > 0)
                {
                    city.AddDoctor(numberToAddOrRemove, 1, 0, false);
                    numberToAddOrRemove--;
                }
                while (numberToAddOrRemove < 0 && city.location.doctors > 0)
                {
                    city.RemoveDoctor();
                    numberToAddOrRemove++;
                }
                if (numberToAddOrRemove == 0)
                {
                    return;
                }
            }
        }
    }

    public void DropDoctor(Vector3 worldLocation, int doctors, float effectiveness = 1, int timeDuration = 0)
    {
        var closeCity = FindCloseCity(worldLocation);
        if (closeCity != null)
        {
            closeCity.AddDoctor(doctors, effectiveness, timeDuration, false);
        }
        else
        {
            // None were nearby, create a new location
            var location = new Location(worldLocation, doctors, effectiveness, timeDuration);
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
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            CastRay();
        }

        if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(TestCityChcking());
    }

    IEnumerator TestCityChcking()
    {
        var iUnityEnumerableEntityGameObject = Instantiate(UnityEngine.Resources.Load("Egg") as GameObject);
        iUnityEnumerableEntityGameObject.transform.SetParent(canvas.transform, false);
        yield return new WaitForSeconds(5);
        Destroy(iUnityEnumerableEntityGameObject);
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
                PlaneBehaviour.SpawnPlane(location.worldLocation, toLocation, dispatchType);
                Resources.Bank.Spend(10000);
                location.TryRemoveCity();
                location.Deselect();
                StartCoroutine(ClearPlaneType());
            }
        }
    }

    IEnumerator ClearPlaneType()
    {
        yield return new WaitForSeconds(0.1f);
        dispatchType = PlaneBehaviour.PlaneType.NONE;
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
