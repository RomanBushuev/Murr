using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Share
{
    public class ShareHistoryCursorDataXml
    {
        [XmlArray("rows")]
        [XmlArrayItem("row")]
        public ShareHistoryBondDataRowXml[] Rows { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
