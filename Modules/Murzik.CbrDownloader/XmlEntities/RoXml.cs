using System;
using System.Linq;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    public class RoXml
    {
        [XmlIgnore()]
        public DateTime D0 { get; set; }

        [XmlElement("D0")]
        public string D0String 
        {
            get { return D0.ToString("yyyy-MM-dd") + "T00:00:00+03:00"; }
            set { D0 = DateTime.ParseExact(new string(value.Take(10).ToArray()), "yyyy-MM-dd", null); }
        }

        [XmlElement("ruo")]
        public decimal Ruo { get; set; }

        [XmlElement("vol")]
        public decimal Vol { get; set; }

        [XmlIgnore()]
        public DateTime DateUpdate { get; set; }

        [XmlElement("DateUpdate")]
        public string DateUpdateString 
        {
            get { return D0.ToString("yyyy-MM-dd") + "T00:00:00+03:00"; }
            set { D0 = DateTime.ParseExact(new string(value.Take(10).ToArray()), "yyyy-MM-dd", null); }
        }
    }
}
