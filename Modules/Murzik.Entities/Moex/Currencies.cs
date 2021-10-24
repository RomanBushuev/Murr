using System;
using System.Collections.Generic;

namespace Murzik.Entities.Moex
{
    public class Currencies
    {
        public DateTime ValidDate { get; set; }

        public IReadOnlyCollection<ValuteCursOnDate> ValuteCursOnDates { get; set; }
    }
}