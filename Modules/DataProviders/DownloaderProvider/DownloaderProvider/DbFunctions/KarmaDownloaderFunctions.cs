using Dapper;
using DownloaderProvider.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DownloaderProvider.DbFunctions
{
    public static class KarmaDownloaderFunctions
    {
        static KarmaDownloaderFunctions()
        {
            FluentMappingInitialize.Initialize();
        }

        public static IEnumerable<DbKarmaDownloadJob> DownloadKarmaDownloadJobs(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_jobs";

            return dbConnection.Query<DbKarmaDownloadJob>(function,
                commandType: CommandType.StoredProcedure);
        }
    }
}
