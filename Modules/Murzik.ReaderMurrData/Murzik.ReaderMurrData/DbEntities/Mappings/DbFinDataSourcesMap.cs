using Dapper.FluentMap.Mapping;

namespace Murzik.ReaderMurrData.DbEntities.Mappings
{
    internal class DbFinDataSourcesMap : EntityMap<DbFinDataSources>
    {
        public DbFinDataSourcesMap()
        {
            Map(p => p.FinDataSoruceId).ToColumn("fin_data_source_id");
            Map(p => p.FinDataSourceIdent).ToColumn("fin_data_source_ident");
        }
    }
}