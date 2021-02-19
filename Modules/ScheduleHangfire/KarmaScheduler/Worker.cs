using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScheduleProvider.DbFunctions;
using System.Data;
using Npgsql;
using System.Diagnostics;
using ScheduleProvider;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace KarmaScheduler
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;

        private Timer _timer;
        private long _interval = 10;
        private string _serviceName = null;
        private ServiceConfig _settings;

        public Worker(ILogger<Worker> logger,
            IOptions<ServiceConfig> smtpSettings)
        {
            _logger = logger;
            _settings = smtpSettings.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running");
            _serviceName = _settings.ServiceName;
            _interval = _settings.Interval;
            SetMessage($"ServiceName: {_serviceName}");
            SetMessage($"{_settings.Configuration}");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            
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

        private string GetStringConnection()
        {
            var npgConnection = _settings.KarmaDownloader;
            return npgConnection;
        }


        private void Job()
        {
            string npgConnection = GetStringConnection();
            DateTime currentDate = DateTime.Now;

            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    //прочитали задачи-процедуры
                    var procedureTasks = KarmaSchedulerFunctions.GetProcedureTasks(connection);

                    //Прочитали все процедуры
                    var procedures = KarmaSchedulerFunctions.GetProcedureInfos(connection);

                    SetMessage($"Прочитали процедур: {procedureTasks.Count()}");

                    //для каждой процедуры смотрим время выполнения
                    foreach (var procedureTask in procedureTasks)
                    {
                        //время выполнения наступило ? 
                        var nextDate = procedureTask.ProcedureNextRun;
                        if (!procedureTask.ProcedureIsUse)
                            continue;

                        if (Utils.MakeNextDate(nextDate, currentDate))
                        {
                            SetMessage($"Прочитали процедуру {procedureTask.ProcedureTitle}");
                            //получили параметры из json
                            var paramValues = Utils.ConvertJsonToParams(procedureTask.ProcedureParams);

                            //параметр и его тип
                            var paramTypes = new Dictionary<string, string>();
                            foreach (var procedure in procedures
                                .Where(z => $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle))
                            {
                                paramTypes[procedure.ParameterName] = procedure.DataType;
                            }

                            //параметры для процедуры
                            var procedureParams = new Dictionary<string, object>();
                            foreach (var paramType in paramTypes)
                            {
                                var param = paramValues[paramType.Key];
                                //Тип параметра не сходится с тем, который указан у нас
                                var isChange = Utils.IsUseTemplate(paramType.Value, param.GetType().ToString());
                                if (isChange)
                                {
                                    var result = Utils.ChangeParmas(param.ToString());
                                    procedureParams[paramType.Key] = result;
                                }
                                else
                                {
                                    procedureParams[paramType.Key] = param;
                                }
                            }

                            string schema = procedures.FirstOrDefault(z => $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle)
                                .ProcedureSchema;

                            SetMessage($"Запустили процедуру {procedureTask.ProcedureTitle}");
                            var isExecuted = KarmaSchedulerFunctions.RunFunction(connection, procedureTask.ProcedureTitle, procedureParams);

                            //изменил значение
                            var lastDate = nextDate;
                            var futureNextDate = Utils.GetNextDateTime(nextDate, currentDate, procedureTask.ProcedureTemplate);

                            procedureTask.ProcedureNextRun = futureNextDate;
                            procedureTask.ProcedureLastRun = lastDate;

                            KarmaSchedulerFunctions.ChangeProcedureTask(connection, procedureTask);
                            SetMessage($"Закончили выполнять процедуру {procedureTask.ProcedureTitle}");
                        }
                    }

                    transaction.Commit();
                }
                connection.Close();
            }
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
