using System;

namespace Murzik.SaverMurrData.DbEntities
{
    public class DbFinDataValue
    {
        public long FinInstrumentId { get; set; }

        public string FinAttributeIdent { get; set; }

        public DateTime Value { get; set; }

        public DateTime FromDate { get; set; }
    }
}
