using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities
{
    [XmlRoot("document")]
    public class DocumentXml
    {
        [XmlElement("data", Order = 1)]
        //[XmlAttribute("id")]
        public HistoryDataXml HistoryData { get; set; }

        [XmlElement("data", Order = 2)]
        public HistoryCursorDataXml HistoryCursorData { get; set; }
    }
}
