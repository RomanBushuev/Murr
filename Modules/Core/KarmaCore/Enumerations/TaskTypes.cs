using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    public enum TaskTypes
    {
        [MurrDbAttribute("UNDEFINED")]
        Undefined = 0,

        [MurrDbAttribute("DOWNLOAD CURRENCIES CBRF")]
        DownloadCurrenciesCbrf = 1,

        [MurrDbAttribute("DOWNLOAD G2 CURVE CBRF")]
        DownloadG2CurveCbrf = 2
    }
}