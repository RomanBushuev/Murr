using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KarmaCore.Calculations;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using KarmaCore.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XmlSaver;

namespace DownloaderService
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        private long _interval = 10;
        private string _serviceName;
        private string currentTaskId = "CURRENT_TASK_ID";
        private string attemptions = "ATTEMPTIONS";
        private ITaskActions _taskActions;
        private IServiceActions _serviceActions;
        private ServiceConfig _settings;
        public TimedHostedService(ILogger<TimedHostedService> logger,
            ITaskActions taskActions,
            IServiceActions serviceActions,
            IOptions<ServiceConfig> smtpSettings)
        {
            _logger = logger;
            _taskActions = taskActions;
            _serviceActions = serviceActions;
            _settings = smtpSettings.Value;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            _serviceName = _settings.ServiceName;
            _interval = _settings.Interval;
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            SetMessage("Run DoWork");
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

        private void Job()
        {
            var services = _serviceActions.GetKarmaServices();
            //проверим, что у сервиса есть
            if (services.FirstOrDefault(z => z.ServiceTitle == _serviceName) == null)
            {
                SetMessage($"Сервис:[{_serviceName}] не найден");
                return;
            }

            //проверим, что сервис рабочий
            if (services.First(z => z.ServiceTitle == _serviceName).ServiceStatus != ServiceStatuses.Running)
            {
                SetMessage($"Сервис:[{_serviceName}] имеет статус {services.First(z => z.ServiceTitle == _serviceName).ServiceStatus.ToDbAttribute()}");
                return;
            }

            decimal? value = _serviceActions.GetNumber(_serviceName, currentTaskId);
            //если работа есть, то проверили попытку выполнить данную работу
            if (value.HasValue && value != -1.0m)
            {
                long numberTaskId = long.Parse(value.Value.ToString());
                //увеличить attemptions 
                decimal? attemption = _taskActions.GetNumber(numberTaskId, attemptions);

                if (!attemption.HasValue)
                {
                    _taskActions.SetAttribute(numberTaskId, attemptions, 1.0m);
                }
                else
                {
                    _taskActions.SetAttribute(numberTaskId, attemptions, attemption.Value + 1);
                }

                //если кол-во > 3 то берем другую задачу
                attemption = _taskActions.GetNumber(numberTaskId, attemptions);

                if (attemption > 3.0m)
                {
                    _serviceActions.SetAttribute(_serviceName, currentTaskId, -1.0m);
                    value = null;
                    //задачу поставить в статус выполнена 
                    _taskActions.ErrorJob(numberTaskId);
                }
            }

            KarmaDownloadJob karmaDownloadJob = null;
            //получили все работы
            if (!value.HasValue || (value.HasValue && value == -1.0m))
            {
                var tasks = _taskActions.GetKarmaDownloadJob();

                if (tasks.Count(z => z.TaskStatuses == TaskStatuses.Created) != 0)
                {
                    SetMessage($"Кол-во найденных работ: {tasks.Count(z => z.TaskStatuses == TaskStatuses.Created)}");
                    foreach (var task in tasks.Where(z => z.TaskStatuses == TaskStatuses.Created))
                    {
                        var result = _taskActions.RunJob(task.TaskId);
                        if (result == 1)
                        {
                            karmaDownloadJob = task;
                            _serviceActions.SetAttribute(_serviceName, currentTaskId, karmaDownloadJob.TaskId);
                            _taskActions.SetAttribute(task.TaskId, attemptions, 1.0m);
                            break;
                        }
                    }
                }
            }

            if (karmaDownloadJob == null)
            {
                _serviceActions.SetAttribute(_serviceName, currentTaskId, -1.0m);
                return;
            }

            SetMessage($"Сервис:{_serviceName} начал работу над {karmaDownloadJob.TaskId}");

            var calculationJson = _taskActions.GetCalculationJson(karmaDownloadJob.TaskTemplateId);
            var calculation = CalculationFactory.GetCalculation(calculationJson);
            calculation.Run();

            if (karmaDownloadJob.SaverTemplateId.HasValue)
            {
                var saverJson = _taskActions.GetSaverJson(karmaDownloadJob.SaverTemplateId.Value);
                var saver = SaverFactory.GetSaver(saverJson, calculation);
                saver.Save();
            }

            _serviceActions.SetAttribute(_serviceName, currentTaskId, -1.0m);
            _taskActions.EndJob(karmaDownloadJob.TaskId);

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
