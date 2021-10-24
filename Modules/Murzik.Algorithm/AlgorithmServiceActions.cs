﻿using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System.Linq;
using System.Threading.Tasks;

namespace Murzik.Algorithm
{
    public class AlgorithmServiceActions : IAlgorithmServiceActions
    {
        private readonly ITaskActions _taskActions;
        private readonly IServiceActions _serviceActions;
        private readonly ICalculationFactory _calculationFactory;
        private readonly ILogger _logger;
        private readonly string _serviceName;

        private string currentTaskId = "CURRENT_TASK_ID";
        private string attemptions = "ATTEMPTIONS";

        public AlgorithmServiceActions(ITaskActions taskActions,
            IServiceActions serviceActions,
            ILogger logger,
            string serviceName)
        {
            _taskActions = taskActions;
            _serviceActions = serviceActions;
            _logger = logger;
            _serviceName = serviceName;
        }

        public Task CheckJob()
        {
            var services = _serviceActions.GetKarmaServices();

            if (services.FirstOrDefault(z => z.ServiceTitle == _serviceName) == null)
            {
                _logger.Info($"Сервис:[{_serviceName}] не найден");
                return Task.CompletedTask;
            }

            if (services.First(z => z.ServiceTitle == _serviceName).ServiceStatus != ServiceStatuses.Running)
            {
                _logger.Info($"Сервис:[{_serviceName}] имеет статус {services.First(z => z.ServiceTitle == _serviceName).ServiceStatus.ToDbAttribute()}");
                return Task.CompletedTask;
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
                    _logger.Info($"Кол-во найденных работ: {tasks.Count(z => z.TaskStatuses == TaskStatuses.Created)}");
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
                return Task.CompletedTask;
            }

            _logger.Info($"Сервис:{_serviceName} начал работу над {karmaDownloadJob.TaskId}");

            var calculationJson = _taskActions.GetCalculationJson(karmaDownloadJob.TaskTemplateId);
            var calculation = _calculationFactory.GetCalculation(calculationJson);
            calculation.Run();

            _serviceActions.SetAttribute(_serviceName, currentTaskId, -1.0m);
            _taskActions.EndJob(karmaDownloadJob.TaskId);

            return Task.CompletedTask;
        }
    }
}
