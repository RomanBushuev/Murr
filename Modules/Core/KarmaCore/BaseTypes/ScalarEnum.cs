using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class ScalarEnum:Scalar<Enum>
    {
        public ScalarEnum(IDictionary<DateTime, Enum> store)
            : base(store)
        {

        }
    }
}
