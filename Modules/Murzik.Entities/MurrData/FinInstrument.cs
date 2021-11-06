using System.Collections.Generic;

namespace Murzik.Entities.MurrData
{
    public class FinInstrument
    {
        public long? FinId { get; set; }

        public string FinIdent { get; set; }

        public long? DataSourceId { get; set; }

        public IReadOnlyCollection<FinDataValue> FinDataValues { get; set; } = new List<FinDataValue>();

        public IReadOnlyCollection<FinNumericValue> FinNumericValues { get; set; } = new List<FinNumericValue>();

        public IReadOnlyCollection<FinStringValue> FinStringValues { get; set; } = new List<FinStringValue>();

        public IReadOnlyCollection<FinTimeSeries> FinTimeSerieses { get; set; } = new List<FinTimeSeries>();
    }
}
