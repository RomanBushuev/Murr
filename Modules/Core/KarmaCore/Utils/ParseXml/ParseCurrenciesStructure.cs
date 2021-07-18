using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace KarmaCore.Utils
{
    public static partial class ParseXmlStructure
    {

        public static Currencies GetCurrencies(XDocument xDocument)
        {
            var element = GetXElement(xDocument, "Currencies");
            if (element != null)
            {
                return GetCurrencies(element);
            }
            else
                return null;
        }

        public static Currencies GetCurrencies(XElement xElement)
        {
            var values = xElement.Elements("ValuteCursOnDate").Select(z => GetValuteCursOnDate(z)).ToList<ValuteCursOnDate>();
            Currencies currencies = new Currencies()
            {
                ValuteCursOnDates = values,
                ValidDate = DateTime.ParseExact(GetXElement(xElement, "ValidDate").Value, "yyyy-MM-dd", null)
            };

            return currencies;
        }

        private static ValuteCursOnDate GetValuteCursOnDate(XElement xElement)
        {
            ValuteCursOnDate valuteCursOnDate = new ValuteCursOnDate()
            {
                Vname = GetXElement(xElement, "Vname").Value.Trim(),
                Vnom = decimal.Parse(GetXElement(xElement, "Vnom").Value, CultureInfo.InvariantCulture),
                Vcurs = decimal.Parse(GetXElement(xElement, "Vcurs").Value, CultureInfo.InvariantCulture),
                Vcode = GetXElement(xElement, "Vcode").Value,
                VchCode = GetXElement(xElement, "VchCode").Value
            };

            return valuteCursOnDate;
        }
    }
}