using System;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    [XmlRoot("Ruonia")]
    public class RuoniaXml
    {
        [XmlIgnore()]
        public DateTime ValidDate { get; set; }

        [XmlElement("ValidDate")]
        public string ValidDateString 
        {
            get { return ValidDate.ToString("yyyy-MM-dd"); }
            set { ValidDate = DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlElement("ro")]
        public RoXml Ro { get; set; }
    }
}
