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
        DownloadG2CurveCbrf = 2,

        [MurrDbAttribute("DOWNLOAD MOSPRIME CBRF")]
        DownloadMosPrimeCbrf = 3,

        [MurrDbAttribute("DOWNLOAD KEYRATE CBRF")]
        DownloadKeyRateCbrf = 4,

        [MurrDbAttribute("DOWNLOAD RUONIA CBRF")]
        DownloadRuoniaCbrf = 5,

        [MurrDbAttribute("DOWNLOAD ROISFIX CBRF")]
        DownloadRoisFixCbrf = 6,

        [MurrDbAttribute("DOWNLOAD MOEX INSTRUMENTS")]
        DownloadMoexInstruments = 7,

        [MurrDbAttribute("SAVE CURRENCIES CBRF")]
        SaveForeignExchange = 8,
    }
}