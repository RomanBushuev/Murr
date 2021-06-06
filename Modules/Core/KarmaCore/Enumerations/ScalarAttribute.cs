using KarmaCore.Utils;

namespace KarmaCore.Enumerations
{
    public enum ScalarAttribute
    {
        Default = 0,
        /// <summary>
        /// Краткое наименование
        /// </summary>
        [MoexFin("SHORTNAME")]
        ShortName = 1,
        /// <summary>
        /// Isin
        /// </summary>
        [MoexFin("ISIN")]
        Isin = 2,
        /// <summary>
        /// Низшая цена
        /// </summary>
        [MoexFin("LOW")]
        Low = 3,
        /// <summary>
        /// Наивысшая цена
        /// </summary>
        [MoexFin("HIGH")]
        High = 4,
        /// <summary>
        /// Ставка
        /// </summary>
        [MoexFin("CLOSE")]
        Close = 5,
        /// <summary>
        /// Цена открытия
        /// </summary>
        [MoexFin("OPEN")]
        Open = 6,
        /// <summary>
        /// Объем
        /// </summary>
        [MoexFin("VOLUME")]
        Volume = 7,
        /// <summary>
        /// Дата погашения
        /// </summary>
        [MoexFin("MATURITY_DATE")]
        MaturityDate = 8,
        /// <summary>
        /// Номинал
        /// </summary>
        [MoexFin("FACEVALUE")]
        FaceValue = 9,
        /// <summary>
        /// Валюта
        /// </summary>
        [MoexFin("CURRENCY")]
        Currency = 10,
        /// <summary>
        /// Наторговали объем на ставку
        /// </summary>
        [MoexFin("TRADED")]
        Traded = 11,
    }
}
