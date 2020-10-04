using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    [Flags]
    public enum ParamType
    {
        Default = 1 << 0,
        Decimal = 1 << 1,
        String = 1 << 2,
        DateTime = 1 << 3
    }
}
