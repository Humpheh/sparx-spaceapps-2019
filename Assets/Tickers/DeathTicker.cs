using System;

namespace mosquitodefenders.Tickers
{
    public class DeathTicker : IResourceTicker<int>
    {
        public int deathToll;

        public DeathTicker()
        {
            deathToll = 0;
        }

        public TickValue<int> NextValue()
        {
            //Debug.LogFormat("Update time {0}", currentDate.ToString());
            //currentDate = currentDate.AddHours(24);
            return new TickValue<int>(Resources.Dead.value);
        }
    }
}
