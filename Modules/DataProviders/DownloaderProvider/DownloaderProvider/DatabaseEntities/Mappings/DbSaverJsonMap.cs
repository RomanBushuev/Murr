using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.DatabaseEntities.Mappings
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
