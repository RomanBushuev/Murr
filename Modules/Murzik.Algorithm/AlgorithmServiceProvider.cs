using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System.Linq;
using System.Threading.Tasks;

namespace Murzik.Algorithm
{
    public class AlgorithmServiceProvider : IAlgorithmServiceProvider
    {
        private readonly ITaskActions _taskActions;
        private readonly IServiceActions _serviceActions;
        private readonly ICalculationFactory _calculationFactory;
        private readonly ILogger _logger;

        private string currentTaskId = "CURRENT_TASK_ID";
        private string attemptions = "ATTEMPTIONS";

        public AlgorithmServiceProvider(ITaskActions taskActions,
            IServiceActions serviceActions,
            ILogger logger,
            ICalculationFactory calculationFactory)
        {
            _taskActions = taskActions;
            _serviceActions = serviceActions;
            _logger = logger;
            _calculationFactory = calculationFactory;
        }

        public async Task CheckJob(string serviceName)
        {             
            var services = _serviceActions.GetKarmaServices();

            if (services.FirstOrDefault(z => z.ServiceTitle == serviceName) == null)
            {
                _logger.Info($"Сервис:[{serviceName}] не найден");
                return;
            }

            if (services.First(z => z.ServiceTitle == serviceName).ServiceStatus != ServiceStatuses.Running)
            {
                _logger.Info($"Сервис:[{serviceName}] имеет статус {services.First(z => z.ServiceTitle == serviceName).ServiceStatus.ToDbAttribute()}");
                return;
            }

            decimal? value = _serviceActions.GetNumber(serviceName, currentTaskId);
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
                    _serviceActions.SetAttribute(serviceName, currentTaskId, -1.0m);
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
                            _serviceActions.SetAttribute(serviceName, currentTaskId, karmaDownloadJob.TaskId);
                            _taskActions.SetAttribute(task.TaskId, attemptions, 1.0m);
                            break;
                        }
                    }
                }
            }

            if (karmaDownloadJob == null)
            {
                _serviceActions.SetAttribute(serviceName, currentTaskId, -1.0m);
                return;
            }

            _logger.Info($"Сервис:{serviceName} начал работу над {karmaDownloadJob.TaskId}");

            var calculationJson = _taskActions.GetCalculationJson(karmaDownloadJob.TaskTemplateId);
            var calculation = _calculationFactory.GetCalculation(calculationJson);
            calculation.TaskId = karmaDownloadJob.TaskId;
            await calculation.Run();

            _serviceActions.SetAttribute(serviceName, currentTaskId, -1.0m);
            _logger.Info($"Сервис:[{serviceName}] закончил проверку работ и уходит в сон");
            return;
        }
    }
}
