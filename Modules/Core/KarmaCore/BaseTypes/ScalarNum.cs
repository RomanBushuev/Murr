using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class ScalarNum:Scalar<decimal>
    {
        public ScalarNum(IDictionary<DateTime, decimal> store)
            : base(store)
        {

        }
    }
}
