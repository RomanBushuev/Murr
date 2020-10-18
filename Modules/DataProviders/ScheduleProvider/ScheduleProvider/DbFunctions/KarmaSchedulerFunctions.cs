using Dapper;
using ScheduleProvider.Mappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScheduleProvider.DbFunctions
{
    public static class KarmaSchedulerFunctions
    {
        static KarmaSchedulerFunctions()
        {
            FluentMappingInitialize.Initialize();
        }

        public static long CreateCbrForeignExchangeDownload(IDbConnection dbConnection,
            CbrForeignParam param)
        {
            string function = "murr_downloader.add_cbr_foreign_exchange";

            return dbConnection.Query<long>(function,
                new { in_datetime = param.DateTime },
                commandType: CommandType.StoredProcedure)
                .First();
        }
    }
}
