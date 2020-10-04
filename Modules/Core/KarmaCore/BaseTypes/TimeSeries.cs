using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class TimeSeries
    {
        public TimeSeries(IDictionary<DateTime, decimal> timeSeries)
        {
            Series = new SortedDictionary<DateTime, decimal>(timeSeries);
        }

        public SortedDictionary<DateTime, decimal> Series { get; } = new SortedDictionary<DateTime, decimal>();

        public void Add(DateTime dateTime, decimal value)
        {
            Series[dateTime] = value;
        }

        public bool HasValue(DateTime dateTime)
        {
            if (Series.ContainsKey(dateTime))
                return true;
            return false;
        }
    }
}
