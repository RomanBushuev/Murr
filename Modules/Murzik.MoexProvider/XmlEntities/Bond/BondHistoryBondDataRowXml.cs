using System.Xml.Serialization;

namespace Murzik.MoexProvider.XmlEntities.Bond
{
    [XmlRoot("row")]
    public class BondHistoryBondDataRowXml
    {
        [XmlAttribute("INDEX")]
        public long Index { get; set; }

        [XmlAttribute("TOTAL")]
        public long Total { get; set; }

        [XmlAttribute("PAGESIZE")]
        public long PageSize { get; set; }
    }
}
