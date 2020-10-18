using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Hangfire.Storage;

namespace KarmaHangfire
{
    public partial class ScheduleJobs : ServiceBase
    {
        private BackgroundJobServer _server;
        private string EveryAt10Hours = "0 8 * * *";

        public ScheduleJobs()
        {
            InitializeComponent();
        }

        public static void AddCbrServiceDownloads()
        {
            CbrServices.DownloadForeignExchange();
        }

        protected override void OnStart(string[] args)
        {
            string connection = ConfigurationManager.AppSettings["karma_admin"];
            GlobalConfiguration.Configuration.UsePostgreSqlStorage(connection);

            using (var hangfireConnection = JobStorage.Current.GetConnection())
            {
                foreach(var recurringJob in hangfireConnection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            _server = new BackgroundJobServer();
            AddCbrServiceDownloads();
        }

        protected override void OnStop()
        {
            _server.Dispose();
        }
    }
}
