using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Share
{
    [XmlRoot("document")]
    public class ShareDocumentXml
    {
        [XmlElement("data", Order = 1)]
        public ShareHistoryDataXml HistoryData { get; set; }

        [XmlElement("data", Order = 2)]
        public ShareHistoryCursorDataXml HistoryCursorData { get; set; }
    }
}
