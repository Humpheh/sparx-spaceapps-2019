using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public class BroadcastingResourceUpdater<T> : ResourceUpdater, IUpdateReceiver<T>
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

            gameObject.BroadcastMessage(topic, newValue);
            foreach (NotifyUpdateDelegate<T> receiver in receivers)
                receiver(newValue);
        }
    }

    public interface IResourceTicker<T>
    {
        T NextValue();
    }

    public delegate void NotifyUpdateDelegate<T>(T value);
}
