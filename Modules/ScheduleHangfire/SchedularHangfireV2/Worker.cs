using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedularHangfireV2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start the service");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string karmaDownloader = System.Configuration.GetSection("DataProviders").GetValue<string>("karma_admin");
                    GlobalConfiguration.Configuration.UsePostgreSqlStorage(connection);

                    using (var hangfireConnection = JobStorage.Current.GetConnection())
                    {
                        foreach (var recurringJob in hangfireConnection.GetRecurringJobs())
                        {
                            RecurringJob.RemoveIfExists(recurringJob.Id);
                        }
                    }

                    _server = new BackgroundJobServer();
                    AddCbrServiceDownloads();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                _logger.LogInformation("Continue the service");
            }
            _logger.LogInformation("Full stop the service");
        }


    }
}
