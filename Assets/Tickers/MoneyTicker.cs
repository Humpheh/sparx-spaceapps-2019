using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;

namespace mosquitodefenders.Tickers
{
    public class MoneyTicker : IResourceTicker<double>
    {
        IMoneyIncrementer incrementer;

        public MoneyTicker(IMoneyIncrementer incrementer)
        {
            this.incrementer = incrementer;
        }

        public TickValue<double> NextValue()
        {
            var nextIncrement = this.incrementer.ShouldIncrement();
            if (nextIncrement == null)
            {
                return new TickValue<double>();
            }
            return new TickValue<double>(nextIncrement.Value);
        }
    }

    public interface IMoneyIncrementer
    {
        double? ShouldIncrement();
    }

    public class MoneyTickCountIncrementer : IMoneyIncrementer
    {
        private readonly uint ticksBetweenIncrements;
        private double incrementBy;

        private uint untilNextIncrement;

        public MoneyTickCountIncrementer(uint ticks, double incrementAmount)
        {
            ticksBetweenIncrements = ticks;
            incrementBy = incrementAmount;
            untilNextIncrement = ticks;
        }

        public Double? ShouldIncrement()
        {
            untilNextIncrement--;
            if (untilNextIncrement == 0)
            {
                untilNextIncrement = ticksBetweenIncrements;
                return incrementBy;
            }
            return null;
        }
    }
}