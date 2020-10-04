using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class ScalarDate : Scalar<DateTime>
    {
        public ScalarDate(IDictionary<DateTime, DateTime> store)
            :base(store)
        {

        }
    }
}
