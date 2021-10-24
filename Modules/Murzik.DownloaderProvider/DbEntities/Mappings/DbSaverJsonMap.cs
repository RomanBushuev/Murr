using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbSaverJsonMap : EntityMap<DbSaverJson>
    {
        public DbSaverJsonMap()
        {
            Map(p => p.JsonParameters).ToColumn("saver_parameters");
            Map(p => p.SaverType).ToColumn("saver_type_id");
        }
    }
}
