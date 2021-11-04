using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Bond
{
    [XmlRoot("document")]
    public class BondDocumentXml
    {
        [XmlElement("data", Order = 1)]
        public BondHistoryDataXml HistoryData { get; set; }

        [XmlElement("data", Order = 2)]
        public BondHistoryCursorDataXml HistoryCursorData { get; set; }
    }
}
