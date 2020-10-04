using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore
{
    public class MarketData
    {
        private IMarketProvider _marketProvider = null;

        public MarketData(IMarketProvider marketProvider)
        {
            _marketProvider = marketProvider;
        }
    }
}
