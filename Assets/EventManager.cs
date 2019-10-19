using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject eventPrefab;

    public float mapWidth = 40;
    public float mapHeight = 30;

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

        if (Random.value < 0.01)
        {
            Vector3 pos = new Vector3(Random.value * mapWidth - mapWidth / 2, Random.value * mapWidth - mapWidth / 2, 0);
            GameObject newObject = Instantiate(eventPrefab, pos, Quaternion.identity);
            newObject.transform.parent = transform;
        }
    }
}
