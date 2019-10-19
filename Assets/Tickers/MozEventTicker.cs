// using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mosquitodefenders.Tickers
{
    public enum MozEventType
    {
        Outbreak = 1,
        Report = 2,
    }

    public class UnknownEventTypeException : Exception { }

    public static class MozEventTypeExtensions
    {
        public static string Name(this MozEventType evt)
        {
            // Don't convert this to an expression
            switch (evt)
            {
                case MozEventType.Outbreak:
                    return "outbreak";
                case MozEventType.Report:
                    return "report";
                default:
                    throw new UnknownEventTypeException();
            }
        }
    }

    public class MozEventTicker : IResourceTicker<MozEvent>
    {
        private readonly MozEventGetter eventGetter;

        public MozEventTicker(MozEventGetter getter)
        {
            eventGetter = getter;
        }

        public TickValue<MozEvent> NextValue()
        {
            var evt = eventGetter.GetEvents();
            var tv = new TickValue<MozEvent>();
            if (evt == null)
            {
                return tv;
            }

            // for now we just take the first event!
            tv.Value = evt.ElementAt(0);
            return tv;
        }
    }


    public struct MozEvent
    {
        public string eventType;
        public DataPoint location;
    }

    public interface MozEventGetter
    {
        List<MozEvent> GetEvents();
    }

    public class MozEventLocalRandom : MozEventGetter
    {
        public static double EventGenerationThresold = 0.05;

        public List<MozEvent> GetEvents()
        {
            if (UnityEngine.Random.value >= 0.05)
            {
                return null;
            }

            DataPoint[] locations = Map.GetSingleton().mozData.points;
            DataPoint location = Utils.RandomInArr(locations);
            MozEventType evtType = Utils.RandomInArr<MozEventType>(
                (ICollection<MozEventType>)Enum.GetValues(typeof(MozEventType))
            );

            MozEvent mozEvent;
            mozEvent.eventType = evtType.Name();
            mozEvent.location = location;

            var lst = new List<MozEvent>();
            lst.Add(mozEvent);
            return lst;
        }
    }
}
