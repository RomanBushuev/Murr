namespace Murzik.Entities.Enumerations
{
    public enum FinAttributes
    {
        [MurrDb("SHORTNAME")]
        ShortNameS = 1,

        [MurrDb("ISIN")]
        IsinS = 2,

        [MurrDb("LOW")]
        LowT = 3,

        [MurrDb("HIGH")]
        HighT = 4,

        [MurrDb("CLOSE")]
        CloseT =5,

        [MurrDb("OPEN")]
        OpenT = 6,

        [MurrDb("VOLUME")]
        VolumeT = 7,

        [MurrDb("MATURITY_DATE")]
        MaturityDateD = 8,

        [MurrDb("FACEVALUE")]
        FaceValueN = 9,

        [MurrDb("CURRENCY")]
        CurrencyS = 10,

        [MurrDb("TRADED")]
        TradedT = 11,

        [MurrDb("TYPE")]
        TypeS = 12,

        [MurrDb("DIGITAL_CODE")]
        DigitalCodeS = 13
    }
}
