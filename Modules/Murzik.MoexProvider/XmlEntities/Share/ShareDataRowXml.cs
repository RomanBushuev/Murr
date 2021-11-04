using Murzik.Utils;
using System;
using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Share
{
    [XmlRoot("row")]
    public class ShareDataRowXml
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
        public decimal? Open { get; set; }

        [XmlAttribute("OPEN")]
        public string OpenString
        {
            get { return Open.ToString(); }
            set { Open = value.ToNullDecimal(); }
        }

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
        public decimal? Legalcloseprice { get; set; }

        [XmlAttribute("LEGALCLOSEPRICE")]
        public string LegalclosepriceString
        {
            get { return Legalcloseprice.ToString(); }
            set { Legalcloseprice = value.ToNullDecimal(); }
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
        public decimal? Close { get; set; }

        [XmlAttribute("CLOSE")]
        public string CloseString
        {
            get { return Close.ToString(); }
            set { Close = value.ToNullDecimal(); }
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

        [XmlAttribute("WAVAL")]
        public decimal Waval { get; set; }

        [XmlAttribute("TRADINGSESSION")]
        public int TradingSession { get; set; }
    }
}
