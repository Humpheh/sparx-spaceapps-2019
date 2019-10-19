using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mosquitodefenders.Tickers;

public class MozEventControl : MonoBehaviour
{
    public GameObject eventPrefab;
    public void MozEvent(MozEvent evt)
    {

        Vector3 pos = new Vector3(
            Map.GetSingleton().GridToMapX((int)evt.location.latitude),
            Map.GetSingleton().GridToMapY((int)evt.location.longitude),
            0
            );
        GameObject newObject = Instantiate(eventPrefab, pos, Quaternion.identity);
        newObject.transform.parent = transform;
        // Create a point for the moz event
    }
}
