using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Utils
{
    public class Currencies
    {
        public DateTime ValidDate { get; set; }

        public IReadOnlyCollection<ValuteCursOnDate> ValuteCursOnDates { get; set; }
    }
}