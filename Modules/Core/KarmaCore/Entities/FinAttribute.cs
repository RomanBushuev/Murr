using KarmaCore.Enumerations;

namespace KarmaCore.Entities
{
    public class FinAttribute
    {
        public long AttributeId { get; set; }

        public string Ident { get; set; }

        public string Title { get; set; }

        public string Descroption { get; set; }

        public FinAttributeType FinAttributeType { get; set; }
    }
}
