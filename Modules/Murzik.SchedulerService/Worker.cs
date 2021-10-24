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
        private readonly ServiceConfig _settings;
        private Timer _timer;

        public Worker(ILogger logger,
            IOptions<ServiceConfig> serviceConfig,
            ISchedulerActions schedulerActions)
        {
            _logger = logger;
            _settings = serviceConfig.Value;
            _schedulerActions = schedulerActions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"������ ������������ ����� ������� {_settings.ServiceName}");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.Interval));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"������ ������������ ����� ���������� {_settings.ServiceName}");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.Info("������ ����� ������������");
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
                _logger.Info("��������� ����� ������������");
                _timer?.Change(TimeSpan.FromSeconds(_settings.Interval), TimeSpan.Zero);
            }
        }
    }
}
