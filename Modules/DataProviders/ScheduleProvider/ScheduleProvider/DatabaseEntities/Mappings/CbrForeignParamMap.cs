using Dapper.FluentMap.Mapping;

namespace ScheduleProvider.Mappings
{
    public class CbrForeignParamMap : EntityMap<DbCbrForeignParam>
    {
        public CbrForeignParamMap()
        {
            Map(p => p.DateTime).ToColumn("in_datetime");
        }
    }
}
