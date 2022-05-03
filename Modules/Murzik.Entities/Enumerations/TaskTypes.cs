namespace Murzik.Entities.Enumerations
{
    public enum TaskTypes
    {
        [MurrDb("UNDEFINED")]
        Undefined = 0,

        [MurrDb("DOWNLOAD CURRENCIES CBRF")]
        DownloadCurrenciesCbrf = 1,

        [MurrDb("DOWNLOAD G2 CURVE CBRF")]
        DownloadG2CurveCbrf = 2,

        [MurrDb("DOWNLOAD MOSPRIME CBRF")]
        DownloadMosPrimeCbrf = 3,

        [MurrDb("DOWNLOAD KEYRATE CBRF")]
        DownloadKeyRateCbrf = 4,

        [MurrDb("DOWNLOAD RUONIA CBRF")]
        DownloadRuoniaCbrf = 5,

        [MurrDb("DOWNLOAD ROISFIX CBRF")]
        DownloadRoisFixCbrf = 6,

        [MurrDb("DOWNLOAD MOEX INSTRUMENTS")]
        DownloadMoexInstruments = 7,

        [MurrDb("SAVE CURRENCIES CBRF")]
        SaveForeignExchange = 8,

        [MurrDb("DOWNLOAD MOEX COUPONS")]
        DownloadMoexCoupons = 9,

        [MurrDb("DOWNLOAD MOEX AMORTIZATIONS")]
        DownloadMoexAmortizations = 10,

        [MurrDb("DOWNLOAD MOEX OFFERS")]
        DownloadMoexOffers = 11,

        [MurrDb("DOWNLOAD MOEX INSTRUMENT DESCIPRTION")]
        DownloadMoexInstrumentDescription = 12,
    }
}
