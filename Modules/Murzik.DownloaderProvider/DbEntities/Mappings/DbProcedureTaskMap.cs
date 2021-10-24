using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbProcedureTaskMap : EntityMap<DbProcedureTask>
    {
        public DbProcedureTaskMap()
        {
            Map(p => p.ProcedureTaskId).ToColumn("procedure_task_id");
            Map(p => p.ProcedureTitle).ToColumn("procedure_title");
            Map(p => p.ProcedureIsUse).ToColumn("procedure_is_use");
            Map(p => p.ProcedureParams).ToColumn("procedure_params");
            Map(p => p.ProcedureTemplate).ToColumn("procedure_template");

            Map(p => p.ProcedureLastRun).ToColumn("procedure_last_run");
            Map(p => p.ProcedureNextRun).ToColumn("procedure_next_run");
        }
    }
}
