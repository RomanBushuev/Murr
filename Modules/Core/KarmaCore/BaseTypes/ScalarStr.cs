using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class ScalarStr : Scalar<string>
    {
        public ScalarStr(IDictionary<DateTime, string> store)
            :base(store)
        {

        }
    }
}
