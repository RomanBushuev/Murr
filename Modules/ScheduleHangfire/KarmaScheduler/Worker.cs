using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Storage;
using ScheduleProvider.DbFunctions;
using System.Data;
using Npgsql;
using ScheduleProvider.Mappings;
using System.Diagnostics;

namespace KarmaScheduler
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;

        private BackgroundJobServer _server;
        private Timer _timer;
        private readonly int _interval = 10;
        private string _serviceName = null;
        private const string AtEvery22 = "* * * * *";

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [AutomaticRetry(Attempts = 0, Order = 1)]
        public static void AddCbrServiceDownloads(Hangfire.Server.PerformContext context,
            string npgConnection)
        {
            try
            {
                using (IDbConnection connection = new NpgsqlConnection(npgConnection))
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        long taskId = KarmaSchedulerFunctions.CreateCbrForeignExchangeDownload(connection, new CbrForeignParam { DateTime = DateTime.Now.AddDays(-1) });
                        transaction.Commit();
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running");
            _logger.LogInformation($">>>>>>>>>> {_configuration.GetValue<string>("Configuration")}");
            _serviceName = _configuration.GetValue<string>("ServiceName");

            SetMessage("Start service");
            SetMessage($"Configuration: {_configuration.GetValue<string>("Configuration")}");
            SetMessage($"ServiceName: {_serviceName}");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            SetMessage("Run DoWork");
            _timer?.Change(Timeout.Infinite, 0);
            if (_server == null)
            {
                SetMessage("Initialize Hangfire");
                string karmaDownloader = _configuration.GetSection("DataProviders").GetValue<string>("karma_admin");
                GlobalConfiguration.Configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UsePostgreSqlStorage(karmaDownloader);

                using (var hangfireConnection = JobStorage.Current.GetConnection())
                {
                    foreach (var recurringJob in hangfireConnection.GetRecurringJobs())
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }

                _server = new BackgroundJobServer();
                RecurringJob.AddOrUpdate("AddCbrServiceDownloads", () => AddCbrServiceDownloads(null, karmaDownloader), AtEvery22);
                SetMessage("Hangfire is initialized");
            }
            SetMessage("End DoWork");
            SetMessage($"{DateTime.Now}");
            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.Zero);
        }

        public void SetMessage(string message)
        {
            if(!string.IsNullOrEmpty(_serviceName))
            {
                if(!EventLog.SourceExists(_serviceName))
                {
                    EventLog.CreateEventSource(_serviceName, _serviceName);
                }
                EventLog eventLog = new EventLog();
                eventLog.Source = _serviceName;

                eventLog.WriteEntry(message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            _server?.SendStop();
            _server?.WaitForShutdown(new TimeSpan(0, 0, 10));
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _server?.Dispose();
        }
    }
}
