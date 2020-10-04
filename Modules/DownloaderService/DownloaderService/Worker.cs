using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DownloaderService
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        private readonly int _interval = 10;

        public TimedHostedService(ILogger<TimedHostedService> logger)
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
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation($"Work: {Thread.GetCurrentProcessorId()}");
            try
            {
                Job();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{ex}");
            }
            finally
            {
                _logger.LogInformation($"Continue the service:{Thread.GetCurrentProcessorId()}");
                _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            }
        }

        private void Job()
        {
            //start job
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
