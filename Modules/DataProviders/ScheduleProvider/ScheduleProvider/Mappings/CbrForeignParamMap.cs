using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleProvider.Mappings
{
    public class CbrForeignParamMap : EntityMap<CbrForeignParam>
    {
        public CbrForeignParamMap()
        {
            Map(p => p.DateTime).ToColumn("in_datetime");
        }
    }
}
