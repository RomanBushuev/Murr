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
using Microsoft.Extensions.Configuration;

namespace TestFullSolutions.PostgresqlFunctions
{
    [TestClass]
    public class TestKarmaDownloaderFunctions
    {

        private string GetStringConnection()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var npgConnection = config
                .GetSection("DataProviders")
                .GetValue<string>("KarmaDownloader");

            return npgConnection;
        }
        [TestMethod]
        public void TestDownloadJobs()
        {
            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                    transaction.Rollback();
                }
            }
        }

    }
}
