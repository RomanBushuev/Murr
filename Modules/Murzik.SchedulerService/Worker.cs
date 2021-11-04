using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Murzik.Entities;
using Murzik.Interfaces;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Murzik.SchedulerService
{
    public class Worker : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ISchedulerActions _schedulerActions;
        private readonly IServiceActions _serviceActions;
        private readonly SchedulerServiceConfige _settings;
        private Timer _timer;

        public Worker(ILogger logger,
            IOptions<SchedulerServiceConfige> serviceConfig,
            ISchedulerActions schedulerActions,
            IServiceActions serviceActions)
        {
            _logger = logger;
            _settings = serviceConfig.Value;
            _schedulerActions = schedulerActions;
            _serviceActions = serviceActions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Сервис палнировщика задач запущен {_settings.ServiceName}");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.Interval));
            _serviceActions.CreateService(_settings.ServiceName);
            _serviceActions.StartService(_settings.ServiceName);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Сервис палнировщика задач остановлен {_settings.ServiceName}");
            _timer?.Change(Timeout.Infinite, 0);
            _serviceActions.StopService(_settings.ServiceName);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.Info("Запуск задач планировщика");
            _timer?.Change(Timeout.Infinite, 0);
            try
            {
                await _schedulerActions.CheckJob();
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _logger.Info("Окончания задач планировщика");
                _timer?.Change(TimeSpan.FromSeconds(_settings.Interval), TimeSpan.Zero);
            }
        }
    }
}
