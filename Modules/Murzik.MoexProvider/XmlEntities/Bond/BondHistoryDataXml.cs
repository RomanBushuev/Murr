using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Bond
{
    public class BondHistoryDataXml
    {
        [XmlArray("rows")]
        [XmlArrayItem("row")]
        public BondDataRowXml[] Rows { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
