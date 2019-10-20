using System;
using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
using UnityEngine;
using UnityEngine.Networking;

public class MozEventCaller : MonoBehaviour
{
    public float nextEvent = 10;
    public int maxIncrement = 10;
    public static string server = "http://localhost:5002";
    System.Random rnd = new System.Random();

    private void OnEvents(List<MozEvent> events)
    {
        foreach (MozEvent @event in events)
        {
            SendMessage("MozEvent", @event);
        }
    }

    // Update is called once per frame
    void Update()
    {
        nextEvent -= Time.deltaTime;
        if (nextEvent <= 0)
        {
            StartCoroutine(DoEvent(OnEvents));
            nextEvent = rnd.Next(1, maxIncrement);
        }
    }
    public delegate void CallbackDelegate(List<MozEvent> evts);

    private IEnumerator DoEvent(CallbackDelegate callback)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{server}/events/new");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            yield break;
        }
        Debug.Log(request.downloadHandler.text);
        Events data = JsonUtility.FromJson<Events>(request.downloadHandler.text);
        callback(new List<MozEvent>(new MozEvent[] { data.events[0].ToMozEvent() }));
    }

    [Serializable]
    public class Events
    {
        public Event[] events;
    }

    [Serializable]
    public class Event
    {
        public double lat;
        public double @long; // @ick
        public int timer;
        public string text;
        public string image_url;
    }
}

static class EventExtensions
{
    public static MozEvent ToMozEvent(this MozEventCaller.Event evt)
    {
        return new MozEvent
        {
            eventType = "report",
            location = new DataPoint
            {
                latitude = (float)evt.lat,
                longitude = (float)evt.@long,
            },
            imageURL = evt.image_url,
            timer = evt.timer,
        };
    }
}
