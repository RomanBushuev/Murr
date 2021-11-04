using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Share
{
    public class ShareHistoryDataXml
    {
        [XmlArray("rows")]
        [XmlArrayItem("row")]
        public ShareDataRowXml[] Rows { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
