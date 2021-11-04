using System;

namespace Murzik.Entities.MoexNew.Bond
{
    public class BondDataRow
    {
        public string Boardid { get; set; }

        public DateTime Tradedate { get; set; }

        public string Shortname { get; set; }

        public string Secid { get; set; }

        public decimal Numtrades { get; set; }

        public decimal Value { get; set; }

        public decimal? Low { get; set; }

        public decimal? High { get; set; }

        public decimal? Close { get; set; }

        public decimal? Legalcloseprice { get; set; }

        public decimal? Accint { get; set; }

        public decimal? Waprice { get; set; }

        public decimal? YieldClose { get; set; }

        public decimal? Open { get; set; }

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

        public DateTime? MatDate { get; set; }

        public decimal? Duration { get; set; }

        public decimal? YieldAtWap { get; set; }

        public decimal? IriCPiClose { get; set; }

        public decimal? BeiClose { get; set; }

        public decimal CouponPercent { get; set; }

        public decimal CouponValue { get; set; }

        public DateTime? BuyBackDate { get; set; }

        public DateTime? LastTradeDate { get; set; }

        public decimal FaceValue { get; set; }

        public string CurrencyId { get; set; }

        public decimal? CbrClose { get; set; }

        public decimal? YieldToOffer { get; set; }

        public decimal? YieldLastCoupon { get; set; }

        public DateTime? OfferDate { get; set; }

        public string FaceUnit { get; set; }

        public int TradingSession { get; set; }
    }
}