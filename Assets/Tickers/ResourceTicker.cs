using UnityEngine;
using System.Collections;

namespace mosquitodefenders.Tickers
{
    public abstract class ResourceUpdater
    {
        protected internal abstract void Tick(GameObject gameObject);
    }

    public class BroadcastingResourceUpdater<T> : ResourceUpdater
    {
        private readonly string topic;
        private readonly IResourceTicker<T> ticker;

        public BroadcastingResourceUpdater(string topic, IResourceTicker<T> ticker)
        {
            this.topic = topic;
            this.ticker = ticker;
        }

        protected internal override void Tick(GameObject gameObject)
        {
            var nextValue = this.ticker.NextValue();
            if (nextValue == null)
            {
                return;
            }
            gameObject.BroadcastMessage(topic, nextValue);
        }
    }

    public interface IResourceTicker<T>
    {
        T NextValue();
    }
}
