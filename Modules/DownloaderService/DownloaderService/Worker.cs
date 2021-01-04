using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DownloaderProvider;
using KarmaCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DownloaderService
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        private readonly int _interval = 10;
        private IConfiguration _configuration;
        private string _serviceName;
        private ITaskActions _taskActions;
        private IServiceActions _serviceActions;

        public TimedHostedService(ILogger<TimedHostedService> logger,
            ITaskActions taskActions,
            IServiceActions serviceActions)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            _serviceName = _configuration.GetValue<string>("ServiceName");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            try
            {
                Job();
            }
            catch (Exception ex)
            {
                SetMessage($"{ex}", "_ERROR");
            }
            finally
            {
                SetMessage("End DoWork");
                SetMessage($"{DateTime.Now}");
                _timer.Change(TimeSpan.FromSeconds(_interval), TimeSpan.Zero);
            }
        }

        private string GetStringConnection()
        {
            var npgConnection = _configuration["DataProviders:KarmaDownloader"];
            return npgConnection;
        }

        private void Job()
        {
            //взять работу на исполнение
            string npgConnection = GetStringConnection();



            //получили все работы 
            _taskActions.GetKarmaDownloadJob();

            //

           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void SetMessage(string message, string postFix = "")
        {
            string generatedService = _serviceName + postFix;
            if (!string.IsNullOrEmpty(generatedService))
            {
                if (!EventLog.SourceExists(generatedService))
                {
                    EventLog.CreateEventSource(generatedService, generatedService);
                }
                EventLog eventLog = new EventLog();
                eventLog.Source = generatedService;

                eventLog.WriteEntry(message);
            }
        }
    }
}
