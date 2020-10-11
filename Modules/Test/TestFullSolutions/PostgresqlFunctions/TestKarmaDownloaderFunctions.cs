using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DownloaderProvider;
using DownloaderProvider.DbFunctions;
using System.Data;
using Npgsql;

namespace TestFullSolutions.PostgresqlFunctions
{
    [TestClass]
    public class TestKarmaDownloaderFunctions
    {
        private string NpgConnection = "User ID=karma_downloader;Password=karma_downloader;Host=localhost;Port=5432;Database=karma;";

        [TestMethod]
        public void TestDownloadJobs()
        {
            using (IDbConnection connection = new NpgsqlConnection(NpgConnection))
            {
                var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
            }
        }

    }
}
