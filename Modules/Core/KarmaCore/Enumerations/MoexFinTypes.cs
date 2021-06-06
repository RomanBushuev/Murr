using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    public enum MoexFinTypes
    {
        [MoexFin("UNDEFINED")]
        Undefined = 0,
        [MoexFin("BOND")]
        Bond = 1,
        [MoexFin("SHARE")]
        Share = 2
    }
}
