using System;
using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
using UnityEngine;
using UnityEngine.Networking;

public class MozEventCaller : MonoBehaviour
{
    public float nextEvent = 10;
    public float maxIncrement = 10;
    System.Random rnd = new System.Random();
    private readonly string server = System.IO.File.ReadAllText("Assets/ngrok.txt").Trim();

    private void OnEvents(List<MozEvent> events)
    {
        foreach (MozEvent @event in events)
        {
            SendMessage("MozEvent", @event);
            StartCoroutine(DoSpreadEvent(OnEvents, @event.timer, @event.location.latitude, @event.location.longitude));

            //this cancels future event spread - dispatch in the actual cure event
            //SendMessage("CuredLocation", new DataPoint
            //{
            //    latitude = @event.location.latitude,
            //    longitude = @event.location.longitude,
            //});
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

        nextEvent -= Time.deltaTime;
        if (nextEvent <= 0)
        {
            StartCoroutine(DoEvent(OnEvents));
            nextEvent = (float)(rnd.NextDouble() * maxIncrement);
        }
    }
    public delegate void CallbackDelegate(List<MozEvent> evts);

    private IEnumerator DoEvent(CallbackDelegate callback)
    {
        Debug.Log($"{server}/events/new");
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
    private IEnumerator DoSpreadEvent(CallbackDelegate callback, float wait, float lat, float @long)
    {
        Debug.LogFormat("Waiting {0} seconds to spread", wait.ToString());
        yield return new WaitForSecondsRealtime(wait);
        foreach (DataPoint loc in Resources.DontSpread)
        {
            if (loc.latitude.Equals(lat) && loc.longitude.Equals(@long))
            {
                // this outbreak has already been cured
                Debug.LogFormat("{0} {1} already cured, stopping", lat.ToString(), @long.ToString());
                yield break;
            }
        }
        Debug.LogFormat("Spreading {0}, {1}", lat.ToString(), @long.ToString());
        UnityWebRequest request = UnityWebRequest.Get($"{server}/events/spread/{lat}/{@long}");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            yield break;
        }
        Debug.Log(request.downloadHandler.text);
        Events data = JsonUtility.FromJson<Events>(request.downloadHandler.text);

        var evts = new List<MozEvent>();
        foreach (Event @event in data.events)
        {
            evts.Add(@event.ToMozEvent());
        }
        callback(evts);
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
            text = evt.text,
        };
    }
}
