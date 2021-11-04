using System;
using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    [XmlRoot("RoisFix")]
    public class RoisFixXml
    {
        [XmlIgnore()]
        public DateTime ValidDate { get; set; }

        [XmlElement("ValidDate")]
        public string ValidDateString 
        {
            get { return ValidDate.ToString("yyyy-MM-dd"); }
            set { ValidDate = DateTime.ParseExact(value, "yyyy-MM-dd", null); }
        }

        [XmlElement("rf")]
        public RfXml Rf { get; set; }
    }
}
