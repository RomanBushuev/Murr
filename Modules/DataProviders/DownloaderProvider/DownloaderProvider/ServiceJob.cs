using DownloaderProvider.DbFunctions;
using DownloaderProvider.Entities;
using KarmaCore.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DownloaderProvider.DatabaseEntities;

namespace DownloaderProvider
{
    public class ServiceJob : IServiceJob
    {
        private readonly string _connection;

        public ServiceJob(string connection)
        {
            _connection = connection;
        }

        public bool RunJob(long jobId)
        {
            return false;
        }

        public List<KarmaDownloadJob> GetKarmaDownloadJob()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                    return ConverterDto.ConvertDto<DbKarmaDownloadJob, KarmaDownloadJob>(result).ToList();
                }
            }
        }        

        public void SetAttribute(long jobId, string attribute, string text)
        {

        }

        public void SetAttribute(long jobId, string attribute, decimal number)
        {

        }

        public void SetAttribute(long jobId, string attribute, DateTime dateTime)
        {

        }

        public void Run()
        {
            throw new NotImplementedException();
        }        
    }
}
