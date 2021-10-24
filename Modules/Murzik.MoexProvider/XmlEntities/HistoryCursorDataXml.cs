using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities
{
    public class HistoryCursorDataXml
    {
        [XmlArray("rows")]
        [XmlArrayItem("row")]
        public HistoryBondDataRowXml[] Rows { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
