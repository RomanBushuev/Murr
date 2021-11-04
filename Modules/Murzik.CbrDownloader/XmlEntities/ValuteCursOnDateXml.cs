using System.Xml.Serialization;

namespace Murzik.CbrDownloader.XmlEntities
{
    public class ValuteCursOnDateXml
    {
        [XmlElement("Vname")]
        public string VName { get; set; }

        [XmlElement("Vnom")]
        public decimal VNom { get; set; }

        [XmlElement("Vcurs")]
        public decimal VCurs { get; set; }

        [XmlElement("Vcode")]
        public decimal Vcode { get; set; }

        [XmlElement("VchCode")]
        public string VchCode { get; set; }
    }
}
