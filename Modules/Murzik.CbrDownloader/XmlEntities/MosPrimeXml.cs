using System;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    [XmlRoot("MosPrime")]
    public class MosPrimeXml
    {
        [XmlIgnore()]
        public DateTime ValidDate { get; set; }

        [XmlElement("ValidDate")]
        public string ValidDateStr
        {
            get { return ValidDate.ToString("yyyy-MM-dd"); }
            set { ValidDate = DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlElement("MP")]
        public MPXml MP { get; set; }
    }
}
