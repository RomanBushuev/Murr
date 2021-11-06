using System.Collections.Generic;

namespace Murzik.Entities.MurrData
{
    public class PackValues
    {
        public IReadOnlyCollection<FinInstrument> FinInstruments { get; set; } = new List<FinInstrument>();
    }
}
