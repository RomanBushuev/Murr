using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Bond
{
    public class BondHistoryCursorDataXml
    {
        [XmlArray("rows")]
        [XmlArrayItem("row")]
        public BondHistoryBondDataRowXml[] Rows { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
