using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
using UnityEngine;

public class MozEventReceiver : MonoBehaviour
{
    public GameObject eventPrefab;
    public void MozEvent(MozEvent evt)
    {
        Debug.Log(evt);
        Vector3 pos = Utils.LatLongToMapCoords(-(int)evt.location.latitude, (int)evt.location.longitude);
        GameObject newObject = Instantiate(eventPrefab, pos, Quaternion.identity);
        newObject.transform.parent = transform;
    }
}
