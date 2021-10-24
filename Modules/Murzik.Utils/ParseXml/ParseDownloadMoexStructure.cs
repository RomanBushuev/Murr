using Murzik.Entities.Moex;
using Murzik.Entities.XmlClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Murzik.Utils.ParseXml
{
    public static partial class ParseXmlStructure
    {
        public static Document GetDocument(XDocument xDocument)
        {
            var element = GetXElement(xDocument, "document");
            if (element != null)
            {
                return GetDocument(element);
            }
            else
                return null;
        }

        public static Document GetDocument(XElement xElement)
        {
            var elements = GetXElements(xElement, "data");
            Document document = new Document()
            {
                Datas = (elements.Count() != 0) ? elements.Select(z => GetDate(z)).ToList() : null
            };

            return document;
        }

        public static Data GetDate(XElement xElement)
        {
            var metaElement = GetXElement(xElement, "metadata");
            var rowsElement = GetXElement(xElement, "rows");
            Data data = new Data()
            {
                Metadata = metaElement != null ? GetMetadata(metaElement) : null,
                Rows = rowsElement != null ? GetRow(rowsElement) : null,
            };

            return data;
        }

        public static Rows GetRow(XElement xElement)
        {
            var count = xElement.Elements("row").Count() == 0;
            if (count)
                return null;

            List<Row> rows = null;
            if (xElement.Elements("row").FirstOrDefault().Attribute("MATDATE") != null)
            {
                rows = xElement.Elements("row").Select(z => GetMoexBondData(z)).ToList<Row>();
            }
            else if (xElement.Elements("row").FirstOrDefault().Attribute("INDEX") != null)
            {
                rows = xElement.Elements("row").Select(z => GetRowMoexMetaRow(z)).ToList<Row>();
            }
            else if (xElement.Elements("row").FirstOrDefault().Attribute("BOARDID") != null)
            {
                rows = xElement.Elements("row").Select(z => GetMoexData(z)).ToList<Row>();
            }

            Rows result = new Rows()
            {
                Rowss = rows
            };

            return result;
        }

        public static MoexMetaRow GetRowMoexMetaRow(XElement xElement)
        {
            MoexMetaRow rowMoexMetaRow = new MoexMetaRow()
            {
                Index = int.Parse(xElement.Attribute("INDEX").Value),
                PageSize = int.Parse(xElement.Attribute("PAGESIZE").Value),
                Total = int.Parse(xElement.Attribute("TOTAL").Value)
            };

            return rowMoexMetaRow;
        }

        public static MoexShareDataRow GetMoexData(XElement xElement)
        {
            try
            {
                MoexShareDataRow rowMoexData = new MoexShareDataRow()
                {
                    Boardid = xElement.Attribute("BOARDID").Value,
                    Tradedate = DateTime.ParseExact(xElement.Attribute("TRADEDATE").Value, "yyyy-MM-dd", null),
                    Shortname = xElement.Attribute("SHORTNAME").Value,
                    Secid = xElement.Attribute("SECID").Value,
                    Numtrades = decimal.Parse(xElement.Attribute("NUMTRADES").Value, CultureInfo.InvariantCulture),
                    Value = decimal.Parse(xElement.Attribute("VALUE").Value, CultureInfo.InvariantCulture),
                    Open = !string.IsNullOrEmpty(xElement.Attribute("OPEN").Value) ? (decimal?)decimal.Parse(xElement.Attribute("OPEN").Value, CultureInfo.InvariantCulture) : null,
                    Low = !string.IsNullOrEmpty(xElement.Attribute("LOW").Value) ? (decimal?)decimal.Parse(xElement.Attribute("LOW").Value, CultureInfo.InvariantCulture) : null,
                    High = !string.IsNullOrEmpty(xElement.Attribute("HIGH").Value) ? (decimal?)decimal.Parse(xElement.Attribute("HIGH").Value, CultureInfo.InvariantCulture) : null,
                    Legalcloseprice = !string.IsNullOrEmpty(xElement.Attribute("LEGALCLOSEPRICE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("LEGALCLOSEPRICE").Value, CultureInfo.InvariantCulture) : null,
                    Waprice = !string.IsNullOrEmpty(xElement.Attribute("WAPRICE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("WAPRICE").Value, CultureInfo.InvariantCulture) : null,
                    Close = !string.IsNullOrEmpty(xElement.Attribute("CLOSE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("CLOSE").Value, CultureInfo.InvariantCulture) : null,
                    Volume = decimal.Parse(xElement.Attribute("VOLUME").Value, CultureInfo.InvariantCulture),
                    Marketprice2 = !string.IsNullOrEmpty(xElement.Attribute("MARKETPRICE2").Value) ? (decimal?)decimal.Parse(xElement.Attribute("MARKETPRICE2").Value, CultureInfo.InvariantCulture) : null,
                    Marketprice3 = !string.IsNullOrEmpty(xElement.Attribute("MARKETPRICE3").Value) ? (decimal?)decimal.Parse(xElement.Attribute("MARKETPRICE3").Value, CultureInfo.InvariantCulture) : null,
                    AdmittedQuote = !string.IsNullOrEmpty(xElement.Attribute("ADMITTEDQUOTE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("ADMITTEDQUOTE").Value, CultureInfo.InvariantCulture) : null,
                    MP2ValTrd = decimal.Parse(xElement.Attribute("MP2VALTRD").Value, CultureInfo.InvariantCulture),
                    Marketprice3TradesValue = decimal.Parse(xElement.Attribute("MARKETPRICE3TRADESVALUE").Value, CultureInfo.InvariantCulture),
                    AdmittedValue = decimal.Parse(xElement.Attribute("ADMITTEDVALUE").Value, CultureInfo.InvariantCulture),
                    WaVal = decimal.Parse(xElement.Attribute("WAVAL").Value, CultureInfo.InvariantCulture),
                    TradingSession = int.Parse(xElement.Attribute("TRADINGSESSION").Value)
                };

                return rowMoexData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static MoexBondDataRow GetMoexBondData(XElement xElement)
        {
            try
            {
                MoexBondDataRow rowMoexData = new MoexBondDataRow()
                {
                    Boardid = xElement.Attribute("BOARDID").Value,
                    Tradedate = DateTime.ParseExact(xElement.Attribute("TRADEDATE").Value, "yyyy-MM-dd", null),
                    Shortname = xElement.Attribute("SHORTNAME").Value,
                    Secid = xElement.Attribute("SECID").Value,
                    Numtrades = decimal.Parse(xElement.Attribute("NUMTRADES").Value, CultureInfo.InvariantCulture),
                    Value = decimal.Parse(xElement.Attribute("VALUE").Value, CultureInfo.InvariantCulture),
                    Open = !string.IsNullOrEmpty(xElement.Attribute("OPEN").Value) ? (decimal?)decimal.Parse(xElement.Attribute("OPEN").Value, CultureInfo.InvariantCulture) : null,
                    Low = !string.IsNullOrEmpty(xElement.Attribute("LOW").Value) ? (decimal?)decimal.Parse(xElement.Attribute("LOW").Value, CultureInfo.InvariantCulture) : null,
                    High = !string.IsNullOrEmpty(xElement.Attribute("HIGH").Value) ? (decimal?)decimal.Parse(xElement.Attribute("HIGH").Value, CultureInfo.InvariantCulture) : null,
                    Legalcloseprice = !string.IsNullOrEmpty(xElement.Attribute("LEGALCLOSEPRICE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("LEGALCLOSEPRICE").Value, CultureInfo.InvariantCulture) : null,
                    Waprice = !string.IsNullOrEmpty(xElement.Attribute("WAPRICE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("WAPRICE").Value, CultureInfo.InvariantCulture) : null,
                    Close = !string.IsNullOrEmpty(xElement.Attribute("CLOSE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("CLOSE").Value, CultureInfo.InvariantCulture) : null,
                    Volume = decimal.Parse(xElement.Attribute("VOLUME").Value, CultureInfo.InvariantCulture),
                    Marketprice2 = !string.IsNullOrEmpty(xElement.Attribute("MARKETPRICE2").Value) ? (decimal?)decimal.Parse(xElement.Attribute("MARKETPRICE2").Value, CultureInfo.InvariantCulture) : null,
                    Marketprice3 = !string.IsNullOrEmpty(xElement.Attribute("MARKETPRICE3").Value) ? (decimal?)decimal.Parse(xElement.Attribute("MARKETPRICE3").Value, CultureInfo.InvariantCulture) : null,
                    AdmittedQuote = !string.IsNullOrEmpty(xElement.Attribute("ADMITTEDQUOTE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("ADMITTEDQUOTE").Value, CultureInfo.InvariantCulture) : null,
                    MP2ValTrd = decimal.Parse(xElement.Attribute("MP2VALTRD").Value, CultureInfo.InvariantCulture),
                    Marketprice3TradesValue = decimal.Parse(xElement.Attribute("MARKETPRICE3TRADESVALUE").Value, CultureInfo.InvariantCulture),
                    AdmittedValue = decimal.Parse(xElement.Attribute("ADMITTEDVALUE").Value, CultureInfo.InvariantCulture),
                    TradingSession = int.Parse(xElement.Attribute("TRADINGSESSION").Value),
                    Accint = !string.IsNullOrEmpty(xElement.Attribute("ACCINT").Value) ? (decimal?)decimal.Parse(xElement.Attribute("ACCINT").Value, CultureInfo.InvariantCulture) : null,
                    CurrencyId = xElement.Attribute("CURRENCYID").Value,
                    FaceValue = !string.IsNullOrEmpty(xElement.Attribute("FACEVALUE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("FACEVALUE").Value, CultureInfo.InvariantCulture) : null,
                    MatDate = !string.IsNullOrEmpty(xElement.Attribute("MATDATE").Value) ? (DateTime?)DateTime.ParseExact(xElement.Attribute("MATDATE").Value, "yyyy-MM-dd", null) : null,
                    YieldClose = !string.IsNullOrEmpty(xElement.Attribute("YIELDCLOSE").Value) ? (decimal?)decimal.Parse(xElement.Attribute("YIELDCLOSE").Value, CultureInfo.InvariantCulture) : null,
                };

                return rowMoexData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Metadata GetMetadata(XElement xElement)
        {
            Metadata metadata = new Metadata()
            {
                Columns = xElement.Element("columns") != null ? GetColumns(xElement.Element("columns")) : null,
            };

            return metadata;
        }

        public static Columns GetColumns(XElement xElement)
        {
            Columns columns = new Columns()
            {
                Column = (xElement.Elements("column").Count() != 0) ? xElement.Elements("column").Select(z => GetColumn(z)).ToList() : null
            };

            return columns;
        }

        public static Column GetColumn(XElement xElement)
        {
            Column columns = new Column()
            {
                Name = xElement.Attribute("name").Value,
                Bytes = xElement.Attribute("bytes") != null ? (int?)int.Parse(xElement.Attribute("bytes").Value) : null,
                MaxSize = xElement.Attribute("max_size") != null ? (int?)int.Parse(xElement.Attribute("max_size").Value) : null,
                Type = xElement.Attribute("type").Value
            };

            return columns;
        }
    }
}
