using DownloaderProvider.Entities;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
            throw new Exception();
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
