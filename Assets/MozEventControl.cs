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
        // Create a point for the moz event
        Vector3 pos = Utils.LatLongToMapCoords((int)evt.location.latitude, (int)evt.location.longitude);

        GameObject newObject = Instantiate(eventPrefab, pos, Quaternion.identity);
        newObject.transform.parent = transform;
        newObject.GetComponent<Outbreak>().evt = evt;
    }
}
