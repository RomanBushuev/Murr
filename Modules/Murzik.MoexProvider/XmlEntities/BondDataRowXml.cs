using Murzik.Utils;
using System;
using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities
{
    [XmlRoot("row")]
    public class BondDataRowXml
    {
        [XmlAttribute("BOARDID")]
        public string Boardid { get; set; }

        [XmlIgnore]
        public DateTime Tradedate { get; set; }

        [XmlAttribute("TRADEDATE")]
        public string TradedateString
        {
            get { return Tradedate.ToString("yyyy-MM-dd"); }
            set { Tradedate = DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }        

        [XmlAttribute("SHORTNAME")]
        public string Shortname { get; set; }

        [XmlAttribute("SECID")]
        public string Secid { get; set; }

        [XmlAttribute("NUMTRADES")]
        public decimal Numtrades { get; set; }

        [XmlAttribute("VALUE")]
        public decimal Value { get; set; }

        [XmlIgnore]
        public decimal? Low { get; set; }

        [XmlAttribute("LOW")]
        public string LowString
        {
            get { return Low.ToString(); }
            set { Low = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? High { get; set; }

        [XmlAttribute("HIGH")]
        public string HighString
        {
            get { return High.ToString(); }
            set { High = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Close { get; set; }

        [XmlAttribute("CLOSE")]
        public string CloseString
        {
            get { return Close.ToString(); }
            set { Close = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Legalcloseprice { get; set; }


        [XmlAttribute("LEGALCLOSEPRICE")]
        public string LegalclosepriceString
        {
            get { return Legalcloseprice.ToString(); }
            set { Legalcloseprice = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Accint { get; set; }

        [XmlAttribute("ACCINT")]
        public string AccintString
        {
            get { return Accint.ToString(); }
            set { Accint = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Waprice { get; set; }

        [XmlAttribute("WAPRICE")]
        public string WapriceString
        {
            get { return Waprice.ToString(); }
            set { Waprice = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? YieldClose { get; set; }

        [XmlAttribute("YIELDCLOSE")]
        public string YieldCloseString
        {
            get { return YieldClose.ToString(); }
            set { YieldClose = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Open { get; set; }

        [XmlAttribute("OPEN")]
        public string OpenString
        {
            get { return Open.ToString(); }
            set { Open = value.ToNullDecimal(); }
        }

        [XmlAttribute("VOLUME")]
        public decimal Volume { get; set; }

        [XmlIgnore]
        public decimal? Marketprice2 { get; set; }

        [XmlAttribute("MARKETPRICE2")]
        public string Marketprice2String
        {
            get { return Marketprice2.ToString(); }
            set { Marketprice2 = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? Marketprice3 { get; set; }

        [XmlAttribute("MARKETPRICE3")]
        public string Marketprice3String
        {
            get { return Marketprice3.ToString(); }
            set { Marketprice3 = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        /// <summary>
        /// Признаваемая котировка
        /// </summary>
        public decimal? AdmittedQuote { get; set; }

        [XmlAttribute("ADMITTEDQUOTE")]
        /// <summary>
        /// Признаваемая котировка
        /// </summary>
        public string AdmittedQuoteString
        {
            get { return AdmittedQuote.ToString(); }
            set { AdmittedQuote = value.ToNullDecimal(); }
        }

        [XmlAttribute("MP2VALTRD")]
        /// <summary>
        /// Объем сделок для расчета Рыночной цены 2
        /// </summary>
        public decimal MP2ValTrd { get; set; }

        [XmlAttribute("MARKETPRICE3TRADESVALUE")]
        public decimal Marketprice3TradesValue { get; set; }

        [XmlAttribute("ADMITTEDVALUE")]
        public decimal AdmittedValue { get; set; }

        [XmlIgnore]
        public DateTime? MatDate { get; set; }

        [XmlAttribute("MATDATE")]
        public string MatDateString
        {
            get { return MatDate.HasValue ? MatDate.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { MatDate = string.IsNullOrEmpty(value) ? null : DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlIgnore]
        public decimal? Duration { get; set; }

        [XmlAttribute("DURATION")]
        public string DurationString
        {
            get { return Duration.ToString(); }
            set { Duration = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? YieldAtWap { get; set; }

        [XmlAttribute("YIELDATWAP")]
        public string YieldAtWapString
        {
            get { return YieldAtWap.ToString(); }
            set { YieldAtWap = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? IriCPiClose { get; set; }

        [XmlAttribute("IRICPICLOSE")]
        public string IriCPiCloseString
        {
            get { return IriCPiClose.ToString(); }
            set { IriCPiClose = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? BeiClose { get; set; }

        [XmlAttribute("BEICLOSE")]
        public string BeiCloseString
        {
            get { return BeiClose.ToString(); }
            set { BeiClose = value.ToNullDecimal(); }
        }

        [XmlAttribute("COUPONPERCENT")]
        public decimal CouponPercent { get; set; }

        [XmlAttribute("COUPONVALUE")]
        public decimal CouponValue { get; set; }

        [XmlIgnore]
        public DateTime? BuyBackDate { get; set; }

        [XmlAttribute("BUYBACKDATE")]
        public string BuyBackDateString
        {
            get { return BuyBackDate.HasValue ? BuyBackDate.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { BuyBackDate = string.IsNullOrEmpty(value) ? null : DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlIgnore]
        public DateTime? LastTradeDate { get; set; }

        [XmlAttribute("LASTTRADEDATE")]
        public string LastTradeDateString
        {
            get { return BuyBackDate.HasValue ? BuyBackDate.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { BuyBackDate = string.IsNullOrEmpty(value) ? null : DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlAttribute("FACEVALUE")]
        public decimal FaceValue { get; set; }

        [XmlAttribute("CURRENCYID")]
        public string CurrencyId { get; set; }

        [XmlIgnore]
        public decimal? CbrClose { get; set; }

        [XmlAttribute("CBRCLOSE")]
        public string CbrCloseString
        {
            get { return CbrClose.ToString(); }
            set { CbrClose = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? YieldToOffer { get; set; }

        [XmlAttribute("YIELDTOOFFER")]
        public string YieldToOfferString
        {
            get { return YieldToOffer.ToString(); }
            set { YieldToOffer = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public decimal? YieldLastCoupon { get; set; }

        [XmlAttribute("YIELDLASTCOUPON")]
        public string YieldLastCouponString
        {
            get { return YieldLastCoupon.ToString(); }
            set { YieldLastCoupon = value.ToNullDecimal(); }
        }

        [XmlIgnore]
        public DateTime? OfferDate { get; set; }

        [XmlAttribute("OFFERDATE")]
        public string OfferDateString
        {
            get { return OfferDate.HasValue ? OfferDate.Value.ToString("yyyy-MM-dd") : string.Empty; }
            set { OfferDate = string.IsNullOrEmpty(value) ? null : DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlAttribute("FACEUNIT")]
        public string FaceUnit { get; set; }

        [XmlAttribute("TRADINGSESSION")]
        public int TradingSession { get; set; }
    }
}
