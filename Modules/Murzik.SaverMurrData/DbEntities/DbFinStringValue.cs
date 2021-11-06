using System;

namespace Murzik.SaverMurrData.DbEntities
{
    public class DbFinStringValue
    {
        public long FinInstrumentId { get; set; }

        public string FinAttributeIdent { get; set; }

        public string Value { get; set; }

        public DateTime FromDate { get; set; }
    }
}
