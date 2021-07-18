using KarmaCore.Utils;

namespace KarmaCore.Enumerations
{
    public enum FinAttribute
    {
        [MurrDbAttribute("SHORTNAME")]
        ShortName = 1,

        [MurrDbAttribute("ISIN")]
        Isin =2,

        [MurrDbAttribute("LOW")]
        Low = 3,

        [MurrDbAttribute("HIGH")]
        High = 4,

        [MurrDbAttribute("CLOSE")]
        Close = 5,

        [MurrDbAttribute("OPEN")]
        Open = 6,

        [MurrDbAttribute("VOLUME")]
        Volume = 7,

        [MurrDbAttribute("MATURITY_DATE")]
        MaturityDate = 8,

        [MurrDbAttribute("FACEVALUE")]
        Facevalue = 9,

        [MurrDbAttribute("CURRENCY")]
        Currency = 10,

        [MurrDbAttribute("TRADED")]
        Traded = 11,

        [MurrDbAttribute("TYPE")]
        Type = 12
    }
}
