// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace mosquitodefenders.Tickers
{
    public struct MozEvent
    {
        public bool infection_risk;
        public DataPoint location;
        public string imageURL;
        public int timer;
        public string text;
    }
}
