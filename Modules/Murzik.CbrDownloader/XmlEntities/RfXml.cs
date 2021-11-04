using System;
using System.Linq;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    public class RfXml
    {
        [XmlIgnore]
        public DateTime D0 { get; set; }

        [XmlElement("D0")]
        public string D0Str
        {
            get { return D0.ToString("yyyy-MM-dd") + "T00:00:00+03:00"; }
            set { D0 = DateTime.ParseExact(new string(value.Take(10).ToArray()), "yyyy-MM-dd", null); }
        }

        [XmlElement("R1W")]
        public decimal R1w { get; set; }

        [XmlElement("R2W")]
        public decimal R2w { get; set; }

        [XmlElement("R1M")]
        public decimal R1m { get; set; }

        [XmlElement("R2M")]
        public decimal R2m { get; set; }

        [XmlElement("R3M")]
        public decimal R3m { get; set; }

        [XmlElement("R6M")]
        public decimal R6m { get; set; }
    }
}
