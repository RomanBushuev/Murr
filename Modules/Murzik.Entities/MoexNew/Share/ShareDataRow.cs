using System;

namespace Murzik.Entities.MoexNew.Share
{
    public class ShareDataRow
    {
        public string Boardid { get; set; }

        public DateTime Tradedate { get; set; }

        public string Shortname { get; set; }

        public string Secid { get; set; }

        public decimal Numtrades { get; set; }

        public decimal Value { get; set; }

        public decimal? Open { get; set; }

        public decimal? Low { get; set; }

        public decimal? High { get; set; }

        public decimal? Legalcloseprice { get; set; }

        public decimal? Waprice { get; set; }

        public decimal? Close { get; set; }

        public decimal Volume { get; set; }

        public decimal? Marketprice2 { get; set; }

        public decimal? Marketprice3 { get; set; }

        /// <summary>
        /// Признаваемая котировка
        /// </summary>
        public decimal? AdmittedQuote { get; set; }

        /// <summary>
        /// Объем сделок для расчета Рыночной цены 2
        /// </summary>
        public decimal MP2ValTrd { get; set; }

        public decimal Marketprice3TradesValue { get; set; }

        public decimal AdmittedValue { get; set; }

        public decimal Waval { get; set; }

        public int TradingSession { get; set; }

    }
}