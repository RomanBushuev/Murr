using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbCalculationJsonMap : EntityMap<DbCalculationJson>
    {
        public DbCalculationJsonMap()
        {
            Map(p => p.TaskTemplate).ToColumn("task_parameters");
            Map(p => p.TaskType).ToColumn("task_type_id");
            Map(p => p.TaskTemplateFolderId).ToColumn("task_template_folder_id");
        }
    }
}
