using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
using UnityEngine;

public class MozEventReceiver : MonoBehaviour
{
    public void MozEvent(MozEvent evt)
    {
        Debug.Log(evt.imageURL);
        Debug.Log(evt.location.latitude);
        Debug.Log(evt.location.longitude);

        if (evt.imageURL.Trim() != "")
        {
            Vector2 position = Utils.LatLongToMapCoords((int)evt.location.latitude, (int)evt.location.longitude);
            Popup.SpawnPanel(position, evt.imageURL, evt.text);
        }
    }
}
