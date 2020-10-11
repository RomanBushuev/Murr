using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.DatabaseEntities.Mappings
{
    public class DbKarmaDownloadJobMap : EntityMap<DbKarmaDownloadJob>
    {
        public DbKarmaDownloadJobMap()
        {
            Map(p => p.TaskId).ToColumn("task_id");
            Map(p => p.TaskTemplateId).ToColumn("task_template_id");
            Map(p => p.TaskStatusId).ToColumn("task_status_id");
        }
    }
}
