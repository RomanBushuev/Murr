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
        DigitalCodeS = 13,

        [MurrDb("ISSUE_DATE")]
        IssueDateD = 14,

        [MurrDb("ISSUE_SIZE")]
        IssueSizeN = 15,

        [MurrDb("REGNUMBER")]
        Regnumber = 16,

        [MurrDb("VALUE_RUB")]
        ValueRubT = 17,

        [MurrDb("NUMTRADES")]
        NumtradesT = 18,

        [MurrDb("WAPRICE")]
        WapriceT = 19,

    }
}
