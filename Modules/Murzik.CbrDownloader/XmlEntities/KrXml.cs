using System;
using System.Linq;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    public class KrXml
    {
        [XmlIgnore]
        public DateTime Dt { get; set; }

        [XmlElement("DT")]
        public string DtStr
        {
            get { return Dt.ToString("yyyy-MM-dd") + "T00:00:00+03:00"; }
            set { Dt = DateTime.ParseExact(new string(value.Take(10).ToArray()), "yyyy-MM-dd", null); }
        }

        [XmlElement("Rate")]
        public decimal Rate { get; set; }
    }
}