using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleProvider.Mappings
{
    public class CbrForeignParam 
    {
        public DateTime DateTime { get; set; }
    }

    public class MoexInstrumentParam
    {
        public DateTime DateTime { get; set; }

        public string InstrumentType { get; set; }
    }
}
