using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
using UnityEngine;

public class MozEventReceiver : MonoBehaviour
{
    public void MozEvent(MozEvent evt)
    {
        Debug.Log(evt);
    }
}
