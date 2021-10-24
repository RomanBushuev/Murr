using Dapper.FluentMap.Mapping;

namespace Murzik.DownloaderProvider.DbEntities.Mappings
{
    public class DbKarmaServiceMap : EntityMap<DbKarmaService>
    {
        public DbKarmaServiceMap()
        {
            Map(p => p.ServiceId).ToColumn("service_id");
            Map(p => p.ServiceTitle).ToColumn("service_title");
            Map(p => p.ServiceStatus).ToColumn("service_status_id");
        }
    }
}