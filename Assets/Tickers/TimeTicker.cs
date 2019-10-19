using System;

namespace mosquitodefenders.Tickers
{
    public class TimeTicker : IResourceTicker<DateTime>
    {
        public DateTime currentDate;

        public TimeTicker()
        {
            currentDate = DateTime.Parse("01/01/2000");
        }

        public TickValue<DateTime> NextValue()
        {
            //Debug.LogFormat("Update time {0}", currentDate.ToString());
            currentDate = currentDate.AddHours(24);
            return new TickValue<DateTime>(currentDate);
        }
    }
}
