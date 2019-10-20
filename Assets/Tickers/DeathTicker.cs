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
            if (Resources.Dead.value > 1000000) GameControl.EndGame();
            return new TickValue<int>(Resources.Dead.value);
        }
    }
}
