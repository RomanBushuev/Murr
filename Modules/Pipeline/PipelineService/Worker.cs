using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineService
{
    public class Worker : IHostedService
    {
        private Timer _timer;
        private Timer _secondTimer;
        private long _interval = 2;
        private long _attemptions = 0;
        private long _secondAttemptions = 0;
        private object _object = 13;

        public Worker(ILogger<Worker> logger)
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            _secondTimer = new Timer(DoSecondWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);

            if (_attemptions == 3)
            {
                Stopping();
            }

            try
            {
                Console.WriteLine("Hello");                
                _attemptions++;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                _timer.Change(TimeSpan.FromSeconds(_interval), TimeSpan.Zero);
            }
        }

        private void DoSecondWork(object state)
        {
            _secondTimer?.Change(Timeout.Infinite, 0);

            if (_secondAttemptions == 3)
            {
                Stopping();
            }

            try
            {
                Console.WriteLine("Second");
                _secondAttemptions++;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _secondTimer.Change(TimeSpan.FromSeconds(_interval), TimeSpan.Zero);
            }
        }

        private void Stopping()
        {
            lock (_object)
            {
                Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }
    }
}
