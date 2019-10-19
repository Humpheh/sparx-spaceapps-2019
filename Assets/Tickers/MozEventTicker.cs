// using System;
using System.Collections;
using UnityEngine;

namespace mosquitodefenders.Tickers
{
    public class MozEventTicker : IResourceTicker<MozEvent?>
    {
        private MozEvent mozEvent;

        private string[] mozEventTypes = { "outbreak", "report" };

        public MozEvent? NextValue()
        {
            if (Random.value < 1)
            {
                DataPoint[] locations = Map.GetSingleton().mozData.points;

                DataPoint location = Utils.RandomInArr(locations);
                string evtType = Utils.RandomInArr(mozEventTypes);

                MozEvent mosEvent;
                mosEvent.eventType = evtType;
                mosEvent.Location = location;

                return mozEvent;
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
        public DataPoint Location;
    }

}
