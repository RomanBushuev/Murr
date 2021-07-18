using Dapper.FluentMap.Mapping;

namespace DownloaderProvider.DatabaseEntities.Mappings
{
    public class DbFinInstrumentMap : EntityMap<DbFinInstrument>
    {
        public DbFinInstrumentMap()
        {
            Map(p => p.FinInstrumentId).ToColumn("fin_instrument_id");
            Map(p => p.DataSourceId).ToColumn("fin_data_source_id");
            Map(p => p.Ident).ToColumn("fin_ident");
        }
    }
}