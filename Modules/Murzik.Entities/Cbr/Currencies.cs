using System;

namespace Murzik.Entities.Cbr
{
    public class Currencies
    {
        public DateTime ValidDate { get; set; }

        public ValuteCursOnDate[] ValuteCursOnDates { get; set; }
    }
}
