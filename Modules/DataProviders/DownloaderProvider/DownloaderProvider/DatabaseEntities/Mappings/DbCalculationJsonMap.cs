using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.DatabaseEntities.Mappings
{
    public class DbCalculationJsonMap : EntityMap<DbCalculationJson>
    {
        public DbCalculationJsonMap()
        {
            Map(p => p.TaskTemplate).ToColumn("task_parameters");
            Map(p => p.TaskType).ToColumn("task_type_id");
        }
    }
}
