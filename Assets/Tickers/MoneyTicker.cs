using UnityEngine;
using System.Collections;

namespace mosquitodefenders.Tickers
{
    public class MoneyTicker : IResourceTicker<double>
    {
        private readonly double increment;

        public MoneyTicker(double incrementAmount)
        {
            increment = incrementAmount;
        }

        public double NextValue()
        {
            return increment;
        }
    }
}