using System;

namespace Murzik.SaverMurrData.DbEntities
{
    public class DbFinTimeSeries
    {
        public long FinInstrumentId { get; set; }

        public string FinAttributeIdent { get; set; }

        public decimal Value { get; set; }

        public DateTime Date { get; set; }
    }
}
