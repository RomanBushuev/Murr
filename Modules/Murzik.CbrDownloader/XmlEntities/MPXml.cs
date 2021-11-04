using System;
using System.Linq;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    public class MPXml
    {
        [XmlIgnore]
        public DateTime MpDate { get; set; }

        [XmlElement("MP_Date")]
        public string MpDateStr
        {
            get { return MpDate.ToString("yyyy-MM-dd") + "T00:00:00+03:00"; }
            set { MpDate = DateTime.ParseExact(new string(value.Take(10).ToArray()), "yyyy-MM-dd", null); }
        }

        [XmlElement("TON")]
        public decimal Ton { get; set; }

        [XmlElement("T1W")]
        public decimal T1w { get; set; }

        [XmlElement("T2W")]
        public decimal T2w { get; set; }

        [XmlElement("T1M")]
        public decimal T1m { get; set; }

        [XmlElement("T2M")]
        public decimal T2m { get; set; }

        [XmlElement("T3M")]
        public decimal T3m { get; set; }

        [XmlElement("T6M")]
        public decimal T4m { get; set; }
    }
}
