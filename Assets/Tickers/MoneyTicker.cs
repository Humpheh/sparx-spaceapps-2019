using UnityEngine;
using System.Collections;

namespace mosquitodefenders.Tickers
{

    public class MoneyTicker : IResourceTicker<double>
    {
        private double balance;
        private readonly double increment;

        public MoneyTicker(double incrementAmount, double initialBalance = 0.0)
        {
            balance = initialBalance;
            increment = incrementAmount;
        }

        public double NextValue()
        {
            balance += increment;
            return balance;
        }
    }
}