﻿using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbKarmaDownloadJobMap : EntityMap<DbKarmaDownloadJob>
    {
        public DbKarmaDownloadJobMap()
        {
            Map(p => p.TaskId).ToColumn("task_id");
            Map(p => p.TaskTemplateId).ToColumn("task_template_id");
            Map(p => p.TaskStatusId).ToColumn("task_status_id");
            Map(p => p.SaverTemplateId).ToColumn("saver_template_id");
        }
    }
}
