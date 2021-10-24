using Murzik.Interfaces;
using System;
using System.Collections.Generic;
using NCrontab;
using Newtonsoft.Json;
using AutoMapper;
using System.Data;
using Npgsql;
using NLog;
using System.Linq;
using System.Threading.Tasks;
using Murzik.DownloaderProvider.DbFunctions;

namespace Murzik.DownloaderProvider
{
    public class SchedulerActions : ISchedulerActions
    {
        private readonly string _connection;
        private ILogger _logger;

        public SchedulerActions(string connection,
            IMapper mapper,
            ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public DateTime ChangeParmas(string value)
        {
            if (value == "now")
                return DateTime.Now;
            if (value == "today")
                return DateTime.Today;
            if (value == "yesterday")
                return DateTime.Today.AddDays(-1);
            if (value == "tomorrow")
                return DateTime.Today.AddDays(1);
            throw new Exception("Error in ChangeParmas");
        }

        public Task CheckJob()
        {
            var currentDate = DateTime.Now;
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    _logger.Info("Получение всех процедур-задач");
                    var procedureTasks = KarmaSchedulerFunctions.GetProcedureTasks(connection);

                    _logger.Info("Получение все процедуры");
                    var procedures = KarmaSchedulerFunctions.GetProcedureInfos(connection);

                    foreach (var procedureTask in procedureTasks)
                    {
                        var nextDate = procedureTask.ProcedureNextRun;
                        if (!procedureTask.ProcedureIsUse)
                            continue;

                        if(MakeNextDate(nextDate, currentDate))
                        {
                            _logger.Info($"Прочитали процедуру {procedureTask.ProcedureTitle}");
                            var paramValues = ConvertJsonToParams(procedureTask.ProcedureParams);

                            var paramTypes = new Dictionary<string, string>();
                            foreach(var procedure in procedures
                                .Where(z => $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle))
                            {
                                paramTypes[procedure.ParameterName] = procedure.DataType;
                            }

                            var procedureParams = new Dictionary<string, object>();

                            foreach (var paramType in paramTypes)
                            {
                                var param = paramValues[paramType.Key];
                                //Тип параметра не сходится с тем, который указан у нас
                                var isChange = IsUseTemplate(paramType.Value, param.GetType().ToString());
                                if (isChange)
                                {
                                    var result = ChangeParmas(param.ToString());
                                    _logger.Info($"Меняем значение {paramType.Key} = {param} => {result}");
                                    procedureParams[paramType.Key] = result;
                                }
                                else
                                {
                                    _logger.Info($"Значение {paramType.Key} = {param}");
                                    procedureParams[paramType.Key] = param;
                                }
                            }

                            string schema = procedures.FirstOrDefault(z => $"{z.ProcedureSchema}.{z.ProcedureName}" == procedureTask.ProcedureTitle)
                                .ProcedureSchema;

                            _logger.Info($"Запустили процедуру {procedureTask.ProcedureTitle}");
                            var isExecuted = KarmaSchedulerFunctions.RunFunction(connection, procedureTask.ProcedureTitle, procedureParams);
                            var lastDate = nextDate;
                            var futureNextDate = GetNextDateTime(nextDate, currentDate, procedureTask.ProcedureTemplate);

                            procedureTask.ProcedureNextRun = futureNextDate;
                            procedureTask.ProcedureLastRun = lastDate;

                            KarmaSchedulerFunctions.ChangeProcedureTask(connection, procedureTask);
                            _logger.Info($"Закончили выполнять процедуру {procedureTask.ProcedureTitle}");
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Dictionary<string, object> ConvertJsonToParams(string json)
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return dictionary;
        }

        public DateTime GetNextDateTime(DateTime? dateTime, DateTime currentDate, string template)
        {
            if (dateTime.HasValue)
            {
                if (MakeNextDate(dateTime, currentDate))
                {
                    var schedule = CrontabSchedule.Parse(template);
                    var nextDate = schedule.GetNextOccurrence(currentDate);
                    return nextDate;
                }
                else
                {
                    return dateTime.Value;
                }
            }
            else
            {
                var schedule = CrontabSchedule.Parse(template);
                var nextDate = schedule.GetNextOccurrence(currentDate);
                return nextDate;
            }
        }

        public bool IsUseTemplate(string postgreSqlType, Type CSharpType)
        {
            return IsUseTemplate(postgreSqlType, CSharpType.ToString());
        }

        public bool IsUseTemplate(string postgreSqlType, string CSharpType)
        {
            if (postgreSqlType == "timestamp without time zone" && CSharpType == "System.String")
                return true;

            return false;
        }

        public bool MakeNextDate(DateTime? nextDate, DateTime currentDate)
        {
            if (!nextDate.HasValue)
                return true;

            return nextDate.Value <= currentDate;
        }
    }
}
