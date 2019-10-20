using UnityEngine;

namespace mosquitodefenders.Tickers
{
    public class CommunityChest : IResourceTicker<bool>
    {
        public TickValue<bool> NextValue()
        {
            if (Random.value < 0.01)
            {
                return new TickValue<bool>(true);
            }
            return new TickValue<bool>(false);
        }
    }
}
