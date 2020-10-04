using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore
{
    public class KarmaEnvironment
    {
        private MarketData _marketDate = null;

        public MarketData Market { get { return _marketDate; } set { _marketDate = value; } }
    }
}
