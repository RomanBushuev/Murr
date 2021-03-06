﻿using System;
using System.Xml.Serialization;

namespace KarmaCore.Utils
{
    [Serializable]
    [XmlRoot("row", Namespace = "")]
    public class MoexShareDataRow : Row
    {
        [XmlElement("BOARDID")]
        public string Boardid { get; set; }

        [XmlElement("TRADEDATE")]
        public DateTime Tradedate { get; set; }

        [XmlElement("SHORTNAME")]
        public string Shortname { get; set; }

        [XmlElement("SECID")]
        public string Secid { get; set; }

        [XmlElement("NUMTRADES")]
        public decimal Numtrades { get; set; }

        [XmlElement("VALUE")]
        public decimal Value { get; set; }

        [XmlElement("OPEN")]
        public decimal? Open { get; set; }

        [XmlElement("LOW")]
        public decimal? Low { get; set; }

        [XmlElement("HIGH")]
        public decimal? High { get; set; }

        [XmlElement("LEGALCLOSEPRICE")]
        public decimal? Legalcloseprice { get; set; }

        [XmlElement("WAPRICE")]
        public decimal? Waprice { get; set; }

        [XmlElement("CLOSE")]
        public decimal? Close { get; set; }

        [XmlElement("VOLUME")]
        public decimal Volume { get; set; }

        [XmlElement("MARKETPRICE2")]
        public decimal? Marketprice2 { get; set; }

        [XmlElement("MARKETPRICE3")]
        public decimal? Marketprice3 { get; set; }

        [XmlElement("ADMITTEDQUOTE")]
        /// <summary>
        /// Признаваемая котировка
        /// </summary>
        public decimal? AdmittedQuote { get; set; }

        [XmlElement("MP2VALTRD")]
        /// <summary>
        /// Объем сделок для расчета Рыночной цены 2
        /// </summary>
        public decimal MP2ValTrd { get; set; }

        [XmlElement("MARKETPRICE3TRADESVALUE")]
        public decimal Marketprice3TradesValue { get; set; }

        [XmlElement("WAVAL")]
        public decimal WaVal { get; set; }

        [XmlElement("ADMITTEDVALUE")]
        public decimal AdmittedValue { get; set; }

        [XmlElement("TRADINGSESSION")]
        public int TradingSession { get; set; }
    }
}
