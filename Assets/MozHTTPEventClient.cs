using System;
using System.Collections.Generic;
using System.Net.Http;
using mosquitodefenders.Tickers;
using UnityEngine;

public class MozHTTPEventClient : MozEventGetter
{
    private string remoteURL;
    private static readonly HttpClient client = new HttpClient();

    public MozHTTPEventClient(string remoteURL)
    {
        this.remoteURL = remoteURL;
    }

    public async List<MozEvent> GetEvents()
    {
        var uri = $"{remoteURL}/events/new";
        HttpResponseMessage resp;
        try
        {
            resp = await client.GetAsync(uri);
            resp.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ev)
        {
            Debug.LogFormat("Problem querying HTTP server for events: {0}", ev.ToString());
        }
    }
}
