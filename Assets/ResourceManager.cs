using UnityEngine;
using System.Collections;
using System;
using mosquitodefenders.Tickers;

static class DefaultResources
{
    readonly public static double DefaultMoney = 10000;
    readonly public static double MoneyIncrement = 1000;
}

static class Resources
{
    public static double money { get; set; }
    public static DateTime time { get; set; }
}

public class ResourceManager : MonoBehaviour
{
    private float ticker = 0;
    private readonly ResourceUpdater[] updaters = null;

    public ResourceManager()
    {
        updaters = new ResourceUpdater[]
        {
            new BroadcastingResourceUpdater<DateTime>("GlobalTimeStep", new TimeTicker()),
            new BroadcastingResourceUpdater<double>("GlobalMoney", new MoneyTicker(
                DefaultResources.MoneyIncrement,
                DefaultResources.DefaultMoney
            ))
        };
    }

    // Update is called once per frame
    void Update()
    {
        ticker += Time.deltaTime;
        if (ticker > 1)
        {
            foreach (ResourceUpdater updater in updaters)
            {
                updater.Tick(gameObject);
            }
            ticker = 0;
        }
    }
}
