using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.DatabaseEntities.Mappings
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