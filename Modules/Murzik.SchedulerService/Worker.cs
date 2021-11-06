using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Murzik.Entities;
using Murzik.Interfaces;
using NLog;
using System;
using System.IO;
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
        private Timer _workTimer;
        private Timer _healthCheckTimer;
        private long _serviceId;

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
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _serviceId = _serviceActions.CreateService(_settings.ServiceName, GetType().Assembly.GetName().Version.ToString());
            _serviceActions.StartService(_settings.ServiceName);

            _workTimer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.Interval));
            _healthCheckTimer = new Timer(HealtCheck, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.Interval));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Сервис палнировщика задач остановлен {_settings.ServiceName}");
            _workTimer?.Change(Timeout.Infinite, 0);
            _healthCheckTimer?.Change(Timeout.Infinite, 0);
            _serviceActions.FinishedService(_settings.ServiceName);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.Info("Запуск задач планировщика");
            _workTimer?.Change(Timeout.Infinite, 0);
            try
            {
                await _schedulerActions.CheckJob();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _logger.Info("Окончания задач планировщика");
                _workTimer?.Change(TimeSpan.FromSeconds(_settings.Interval), TimeSpan.Zero);
            }
        }

        private async void HealtCheck(object state)
        {
            var date = DateTime.Now;
            _logger.Info($"Отправка HealthCheck  для сервиса {_serviceId}:{date}");
            _healthCheckTimer?.Change(Timeout.Infinite, 0);
            try
            {
                await _serviceActions.SetHealthCheckAsync(_serviceId, date);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _healthCheckTimer?.Change(TimeSpan.FromSeconds(_settings.Interval), TimeSpan.Zero);
            }
        }
    }
}
