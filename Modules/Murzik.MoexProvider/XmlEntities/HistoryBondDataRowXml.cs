using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities
{
    [XmlRoot("row")]
    public class HistoryBondDataRowXml
    {
        [XmlAttribute("INDEX")]
        public long Index { get; set; }

        [XmlAttribute("TOTAL")]
        public long Total { get; set; }

        [XmlAttribute("PAGESIZE")]
        public long PageSize { get; set; }
    }
}
