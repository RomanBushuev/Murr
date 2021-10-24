using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbProcedureInfoMap : EntityMap<DbProcedureInfo>
    {
        public DbProcedureInfoMap()
        {
            Map(p => p.DataType).ToColumn("data_type");
            Map(p => p.ExternalLanguage).ToColumn("external_language");
            Map(p => p.ParameterMode).ToColumn("pm");
            Map(p => p.ParameterName).ToColumn("pn");
            Map(p => p.ProcedureName).ToColumn("procedure_name");
            Map(p => p.ProcedureNameSecond).ToColumn("procedure_name_second");
            Map(p => p.ProcedureSchema).ToColumn("procedure_schema");
        }
    }
}
