using System;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    [XmlRoot("KeyRate")]
    public class KeyRateXml
    {
        [XmlIgnore]
        public DateTime ValidDate { get; set; }

        [XmlElement("ValidDate")]
        public string ValidDateStr
        {
            get { return ValidDate.ToString("yyyy-MM-dd"); }
            set { ValidDate = DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlElement("KR")]
        public KrXml Kr { get; set; }
    }
}
