// using System;
using System.Collections;
using UnityEngine;

namespace mosquitodefenders.Tickers
{
    public class MozEventTicker : IResourceTicker<MozEvent>
    {
        private string[] mozEventTypes = { "outbreak", "report" };

        public TickValue<MozEvent> NextValue()
        {
            if (Random.value < 1)
            {
                DataPoint[] locations = Map.GetSingleton().mozData.points;
                DataPoint location = Utils.RandomInArr(locations);
                string evtType = Utils.RandomInArr(mozEventTypes);

                MozEvent mozEvent;
                mozEvent.eventType = evtType;
                mozEvent.location = location;

                return new TickValue<MozEvent>(mozEvent);
            }
            else
            {
                return null;
            }
        }
    }


    public struct MozEvent
    {
        public string eventType;
        public DataPoint location;
    }

    public interface MozEventGetter
    {

    }
}
