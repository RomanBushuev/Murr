using Dapper.FluentMap.Mapping;

namespace DownloaderProvider.DatabaseEntities.Mappings
{
    public class DbDataSourceMap : EntityMap<DbFinDataSource>
    {
        public DbDataSourceMap()
        {
            Map(p => p.FinDataSourceId).ToColumn("fin_data_source_id");
            Map(p => p.FinDataSourceIdent).ToColumn("fin_data_source_ident");
        }
    }
}
