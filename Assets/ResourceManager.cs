﻿using UnityEngine;
using System.Collections;
using System;
using mosquitodefenders.Tickers;
using System.Runtime.CompilerServices;

static class DefaultResources
{
    readonly public static double DefaultMoney = 10000000;
    readonly public static double MoneyIncrement = 1000;
    readonly public static int StartLevel = 1;
}

internal class ResourceTracker<T>
{
    private T _value;
    public T value
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get { return _value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set { _value = value; }
    }

}

static class Resources
{
    public static Bank Bank { get; } = new Bank(DefaultResources.DefaultMoney);
    public static ResourceTracker<DateTime> Time { get; } = new ResourceTracker<DateTime>();
    public static ResourceTracker<int> Level { get; } = new ResourceTracker<int>();
}

public class ResourceManager : MonoBehaviour
{
    private float ticker = 0;
    private readonly ResourceUpdater[] updaters = null;

    public ResourceManager()
    {
        var MoneyTicker = new BroadcastingResourceUpdater<double>("GlobalMoney", new MoneyTicker(
            DefaultResources.MoneyIncrement
        ));
        MoneyTicker.RegisterReceiver(Resources.Bank.Add);

        updaters = new ResourceUpdater[]
        {
            new BroadcastingResourceUpdater<DateTime>("GlobalTimeStep", new TimeTicker()),
            new BroadcastingResourceUpdater<MozEvent>("MozEvent", new MozEventTicker()),
            MoneyTicker
        };
    }

    void Start()
    {
        Resources.Level.value = DefaultResources.StartLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

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
