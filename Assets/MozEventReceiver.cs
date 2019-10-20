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
        Vector3 pos = Utils.ApiEvtToMapCoords(evt.location);
        GameObject newObject = Instantiate(eventPrefab, pos, Quaternion.identity);
        newObject.transform.parent = transform;
        newObject.GetComponent<Outbreak>().eventType = evt.eventType;
        newObject.GetComponent<Outbreak>().evt = evt;
        // Debug.Log(evt.eventType);

        Debug.Log(evt.imageURL);
        // Debug.Log(evt.location.latitude);
        // Debug.Log(evt.location.longitude);

        if (evt.imageURL.Trim() != "")
        {
            Vector3 position = Utils.ApiEvtToMapCoords(evt.location);
            Popup.SpawnPanel((Vector2)position, evt.imageURL, evt.text);
        }
    }
}
