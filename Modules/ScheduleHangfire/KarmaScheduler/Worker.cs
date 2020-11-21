using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ScheduleProvider.DbFunctions;
using System.Data;
using Npgsql;
using System.Diagnostics;
using ScheduleProvider;
using System.Collections.Generic;
using System.Linq;

namespace KarmaScheduler
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;

        private Timer _timer;
        private readonly int _interval = 10;
        private string _serviceName = null;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running");
            _logger.LogInformation($">>>>>>>>>> {_configuration.GetValue<string>("Configuration")}");
            _serviceName = _configuration.GetValue<string>("ServiceName");

            SetMessage("Start service");
            SetMessage($"Configuration: {_configuration.GetValue<string>("Configuration")}");
            SetMessage($"ServiceName: {_serviceName}");

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
            catch(Exception ex)
            {
                SetMessage($"{ex}");
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
            var npgConnection = _configuration["DataProviders:KarmaDownloader"];
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
                    //прочитали процедуру
                    var procedureTasks = KarmaSchedulerFunctions.GetProcedureTasks(connection);

                    //Прочитали все процедуры
                    var procedures = KarmaSchedulerFunctions.GetProcedureInfos(connection);

                    foreach (var procedureTask in procedureTasks)
                    {
                        var nextDate = procedureTask.ProcedureNextRun;

                        if (procedureTask.ProcedureIsUse)
                            continue;
                        
                        if(Utils.MakeNextDate(nextDate, currentDate))
                        {
                            SetMessage($"Прочитали процедуру {procedureTask.ProcedureTitle}");
                            //запустить процедуру
                            var keyValuePairs = Utils.ConvertToParams(procedureTask.ProcedureParams);

                            var paramTypes = new Dictionary<string, string>();
                            foreach (var procedure in procedures
                                .Where(z=> $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle))
                            {
                                paramTypes[procedure.ParameterName] = procedure.DataType;
                            }

                            var resultKeyValues = new Dictionary<string, object>();
                            foreach(var paramType in paramTypes)
                            {
                                var param = keyValuePairs[paramType.Key];
                                var isChange = Utils.IsUseTemplate(paramType.Value, param.GetType().ToString());
                                if (isChange)
                                {
                                    var result = Utils.ChangeParmas(param.ToString());
                                    resultKeyValues[paramType.Key] = result;
                                }
                                else
                                {
                                    resultKeyValues[paramType.Key] = paramType.Value;
                                }
                            }

                            string schema = procedures.FirstOrDefault(z => $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle)
                                .ProcedureSchema;

                            IDbConnection other = new NpgsqlConnection(npgConnection);
                            SetMessage($"Запустили процедуру {procedureTask.ProcedureTitle}");
                            bool isExecuted = KarmaSchedulerFunctions.RunFunction(other, procedureTask.ProcedureTitle, resultKeyValues);

                            //изменил значение
                            var lastDate = nextDate;
                            var futureNextDate = Utils.GetNextDateTime(nextDate, currentDate, procedureTask.ProcedureTemplate);

                            procedureTask.ProcedureNextRun = futureNextDate;
                            procedureTask.ProcedureLastRun = lastDate;

                            KarmaSchedulerFunctions.ChangeProcedureTask(connection, procedureTask);
                            SetMessage($"Закончили выполнять процедуру {procedureTask.ProcedureTitle}");

                            transaction.Commit();
                        }
                    }
                }
            }
        }

        public void SetMessage(string message)
        {
            if(!string.IsNullOrEmpty(_serviceName))
            {
                if(!EventLog.SourceExists(_serviceName))
                {
                    EventLog.CreateEventSource(_serviceName, _serviceName);
                }
                EventLog eventLog = new EventLog();
                eventLog.Source = _serviceName;

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
