using System;

namespace Murzik.SaverMurrData.DbEntities
{
    public class DbFinNumericValue
    {
        public long FinInstrumentId { get; set; }

        public string FinAttributeIdent { get; set; }

        public decimal Value { get; set; }

        public DateTime FromDate { get; set; }
    }
}
