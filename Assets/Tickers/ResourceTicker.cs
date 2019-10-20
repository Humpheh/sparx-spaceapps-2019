using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace mosquitodefenders.Tickers
{
    public abstract class ResourceUpdater
    {
        protected internal abstract void Tick(GameObject gameObject);
    }

    interface IUpdateReceiver<T>
    {
        void RegisterReceiver(NotifyUpdateDelegate<T> receiver);
    }

    public class BroadcastingResourceUpdater<T> :
        ResourceUpdater, IUpdateReceiver<T>
        where T : struct
    {
        private readonly string topic;
        private readonly IResourceTicker<T> ticker;
        private readonly List<NotifyUpdateDelegate<T>> receivers = new List<NotifyUpdateDelegate<T>>();

        public BroadcastingResourceUpdater(string topic, IResourceTicker<T> ticker)
        {
            this.topic = topic;
            this.ticker = ticker;
        }

        public void RegisterReceiver(NotifyUpdateDelegate<T> receiver)
        {
            receivers.Add(receiver);
        }

        protected internal override void Tick(GameObject gameObject)
        {
            var newValue = ticker.NextValue();
            if (newValue == null)
            {
                return;
            }
            if (!newValue.Valid)
            {
                return;
            }
            SendEvent(gameObject, newValue.Value);
        }

        private void SendEvent(GameObject gameObject, T v)
        {
            gameObject.BroadcastMessage(topic, v);
            foreach (NotifyUpdateDelegate<T> receiver in receivers)
                receiver(v);
        }
    }

    public interface IResourceTicker<T> where T : struct
    {
        TickValue<T> NextValue();
    }

    public delegate void NotifyUpdateDelegate<T>(T value);
}

public class TickValue<T>
{
    protected internal bool Valid;
    private T _value;
    protected internal T Value
    {
        get
        {
            return _value;
        }

        set
        {
            _value = value;
            Valid = _value != null;
        }
    }

    protected internal TickValue(T Value)
    {
        this.Value = Value;
    }

    protected internal TickValue() { }
}
