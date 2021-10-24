using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Murzik.Entities;
using Murzik.Interfaces;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Murzik.AlgorithmService
{
    public class Worker : IHostedService
    {
        private ILogger _logger;
        private IAlgorithmServiceActions _algorithmServerActions;
        private ServiceConfig _settings;
        private Timer _timer;

        public Worker(ILogger logger,
            IAlgorithmServiceActions algorithmServerActions,
            IOptions<ServiceConfig> settings)
        {
            _logger = logger;
            _algorithmServerActions = algorithmServerActions;
            _settings = settings.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Сервис выполнения алгоритмов запущен {_settings.ServiceName}");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.Interval));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Сервис выполнения алгоритмов остановлен {_settings.ServiceName}");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private async void DoWork(object state)
        {
            _logger.Info("Запуск задач планировщика");
            _timer?.Change(Timeout.Infinite, 0);
            try
            {
                await _algorithmServerActions.CheckJob();
            }
            catch (Exception ex)
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
