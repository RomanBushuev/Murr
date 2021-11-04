using System;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    [XmlRoot("Currencies")]
    public class CurrenciesXml
    {
        [XmlIgnore]
        public DateTime ValidDate { get; set; }

        [XmlElement("ValidDate")]
        public string ValidDateStr
        {
            get { return ValidDate.ToString("yyyy-MM-dd"); }
            set { ValidDate = DateTime.ParseExact(new string(value), "yyyy-MM-dd", null); }
        }

        [XmlElement("ValuteCursOnDate")]
        public ValuteCursOnDateXml[] ValuteCursOnDates { get; set; }
    }
}
