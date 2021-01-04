using KarmaCore.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DownloaderProvider;
using DownloaderProvider.DbFunctions;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using ScheduleProvider.DbFunctions;
using ScheduleProvider.Mappings;
using System;
using DownloaderProvider.DatabaseEntities;
using KarmaCore.Entities;
using ScheduleProvider;
using KarmaCore.Interfaces;
using KarmaCore.Utils;

namespace TestFullSolutions.PostgresqlFunctions
{
    [TestClass]
    public class TestKarmaDownloaderFunctions
    {
        private string GetStringConnection()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            if (!config.GetSection("DataProviders").Exists())
                throw new Exception("Отсутствует DataProviders");

            var npgConnection = config["DataProviders:KarmaDownloader"];

            return npgConnection;
        }
        [TestMethod]
        public void TestDownloadJobs()
        {
            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                    transaction.Rollback();
                }
            }
        }

        [TestMethod]
        public void TestCreateCbrDownloadTasks()
        {
            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    long taskId = KarmaSchedulerFunctions.CreateCbrForeignExchangeDownload(connection, new CbrForeignParam { DateTime = DateTime.Now });
                    var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                    Assert.IsNotNull(result.FirstOrDefault(z => z.TaskId == taskId));
                    transaction.Rollback();
                }
            }
        }

        [TestMethod]
        public void TestCreateCbrDownloadTasksAndChangeStatus()
        {
            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    long taskId = KarmaSchedulerFunctions.CreateCbrForeignExchangeDownload(connection, new CbrForeignParam { DateTime = DateTime.Now });
                    var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                    Assert.IsNotNull(result.FirstOrDefault(z => z.TaskId == taskId));
                    Assert.IsTrue(result.FirstOrDefault(z => z.TaskId == taskId).TaskStatusId == (long)TaskStatuses.Created);

                    var isChanged = KarmaDownloaderFunctions.ChangeTaskStatus(connection, taskId, (long)TaskStatuses.Created, (long)TaskStatuses.Running);
                    Assert.IsTrue(isChanged == 1);
                    transaction.Rollback();
                }
            }
        }

        [TestMethod]
        public void TestAddDate()
        {
            string attemptions = "ATTEMPTIONS";
            string log = "INFO_LOG";
            string startTask = "START_TASK";

            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    long taskId = KarmaSchedulerFunctions.CreateCbrForeignExchangeDownload(connection, new CbrForeignParam { DateTime = DateTime.Now });

                    DateTime dateTime = DateTime.Now;
                    KarmaDownloaderFunctions.InsertTaskNumeric(connection, taskId, attemptions, 2);
                    KarmaDownloaderFunctions.InsertTaskDate(connection, taskId, startTask, dateTime);
                    KarmaDownloaderFunctions.InsertTaskDateText(connection, taskId, log, dateTime, "Hello");

                    var attemps = KarmaDownloaderFunctions.GetTaskDecimal(connection, taskId, attemptions);
                    Assert.AreEqual(2.0m, attemps);

                    var start = KarmaDownloaderFunctions.GetTaskDate(connection, taskId, startTask);
                    Assert.AreEqual(dateTime.ToString(), start.ToString());

                    transaction.Rollback();
                }
            }
        }

        [TestMethod]
        public void TestGetProcedures()
        {
            string npgSqlConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgSqlConnection))
            {
                connection.Open();

                var procedures = KarmaSchedulerFunctions.GetProcedureInfos(connection);
                Assert.IsTrue(procedures.Count() != 0);
            }
        }

        /// <summary>
        /// Запус процедуры
        /// </summary>
        [TestMethod]
        public void TestUseTemplates(bool save = false)
        {
            string npgSqlConnection = GetStringConnection();
            string otherSqlConnection = GetStringConnection();
            IDbConnection other = new NpgsqlConnection(npgSqlConnection);
            using (IDbConnection connection = new NpgsqlConnection(npgSqlConnection))
            {
                connection.Open();
                other.Open();
                var transaction = other.BeginTransaction();

                var procedures = KarmaSchedulerFunctions.GetProcedureInfos(connection);
                string function = "add_cbr_foreign_exchange";
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>() { ["in_datetime"] = "now" };

                Dictionary<string, string> paramTypes = new Dictionary<string, string>();
                foreach(var procedure in procedures.Where(z => z.ProcedureName == function))
                {
                    paramTypes[procedure.ParameterName] = procedure.DataType;
                }

                Dictionary<string, object> resultKeyValues = new Dictionary<string, object>();
                foreach (var paramType in paramTypes)
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

                var preciosionNow = DateTime.Now;
                var value = (DateTime)resultKeyValues["in_datetime"];
                Assert.IsTrue(value >= preciosionNow.AddSeconds(-1) && value <= preciosionNow.AddSeconds(+1));

                string schema = procedures.FirstOrDefault(z => z.ProcedureName == function).ProcedureSchema;
                bool isExecuted = KarmaSchedulerFunctions.RunFunction(other, $"{schema}.{function}", resultKeyValues);
                Assert.IsTrue(isExecuted);

                if (save)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        [TestMethod]
        public void TestGetProcedureTasks()
        {
            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();                
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    ProcedureTask procedureTask = new ProcedureTask()
                    {
                        ProcedureTitle = "murr_downloader.add_cbr_foreign_exchange",
                        ProcedureIsUse = true,
                        ProcedureParams = "{\"in_datetime\": \"now\"}",
                        ProcedureTemplate = "* * * * *",
                        ProcedureLastRun = null,
                        ProcedureNextRun = null
                    };

                    //вставили процедуру 
                    var id = KarmaSchedulerFunctions.InsertProcedureTask(connection, procedureTask);
                    procedureTask.ProcedureTaskId = id;

                    //прочитали процедуру
                    var result = KarmaSchedulerFunctions.GetProcedureTasks(connection);
                    Assert.IsNotNull(result.FirstOrDefault(z => z.ProcedureTaskId == id));

                    //изменили значение процедуру
                    procedureTask.ProcedureLastRun = null;
                    procedureTask.ProcedureNextRun = new DateTime(2020, 11 , 17, 12 ,00 ,00);                    
                    KarmaSchedulerFunctions.ChangeProcedureTask(connection, procedureTask);

                    //прочитали процедуру
                    result = KarmaSchedulerFunctions.GetProcedureTasks(connection);
                    Assert.IsNull(result.FirstOrDefault(z => z.ProcedureTaskId == id).ProcedureLastRun);
                    Assert.AreEqual(procedureTask.ProcedureNextRun, result.FirstOrDefault(z => z.ProcedureTaskId == id).ProcedureNextRun);

                    procedureTask.ProcedureLastRun = new DateTime(2020, 11, 17, 12, 00, 00);
                    procedureTask.ProcedureNextRun = new DateTime(2020, 11, 17, 12, 00, 01);
                    KarmaSchedulerFunctions.ChangeProcedureTask(connection, procedureTask);

                    //прочитали процедуру
                    result = KarmaSchedulerFunctions.GetProcedureTasks(connection);
                    Assert.AreEqual(procedureTask.ProcedureLastRun, result.FirstOrDefault(z => z.ProcedureTaskId == id).ProcedureLastRun);
                    Assert.AreEqual(procedureTask.ProcedureNextRun, result.FirstOrDefault(z => z.ProcedureTaskId == id).ProcedureNextRun);

                    transaction.Rollback();
                }
            }
        }

        [TestMethod]
        public void TestMapping()
        {
            DbKarmaDownloadJob dbKarmaDownloadJob = new DbKarmaDownloadJob()
            {
                TaskId = 1,
                TaskStatusId = 2,
                TaskTemplateId = 1
            };

            KarmaDownloadJob karmaDownloadJob = ConverterDto.ConvertDto<DbKarmaDownloadJob, KarmaDownloadJob>(dbKarmaDownloadJob);
            Assert.AreEqual(karmaDownloadJob.TaskStatuses, TaskStatuses.Created);
            Assert.AreEqual(karmaDownloadJob.TaskId, 1);
            Assert.AreEqual(karmaDownloadJob.TaskTemplateId, 1);

            dbKarmaDownloadJob = ConverterDto.ConvertDto<KarmaDownloadJob, DbKarmaDownloadJob>(new KarmaDownloadJob()
            { 
                TaskStatuses = TaskStatuses.Done,
                TaskId = 2,
                TaskTemplateId = 2
            });

            Assert.AreEqual(dbKarmaDownloadJob.TaskStatusId, 4);
            Assert.AreEqual(dbKarmaDownloadJob.TaskId, 2);
            Assert.AreEqual(dbKarmaDownloadJob.TaskTemplateId, 2);
        }

        [TestMethod]
        public void TestServiceAttributes()
        {
            string alias = "ALIAS";
            string currentTaskId = "CURRENT_TASK_ID";
            string log = "SERVICE_LOG";
            string lastWorkingDateTime = "LAST_WORKING_DATE_TIME";
            string serviceName = "BushuevService";
            DateTime date = DateTime.Now;

            string npgConnection = GetStringConnection();
            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                connection.Open();                
                var transaction = connection.BeginTransaction();

                long serviceId = KarmaDownloaderFunctions.CreateService(connection, serviceName);

                KarmaDownloaderFunctions.InsertServiceDate(connection, serviceName, lastWorkingDateTime, date);
                KarmaDownloaderFunctions.InsertServiceNumeric(connection, serviceName, currentTaskId, 1);
                KarmaDownloaderFunctions.InsertServiceString(connection, serviceName, alias, "Roman");
                KarmaDownloaderFunctions.InsertServiceDateString(connection, serviceName, log, date, "Roman");

                var actualDate = KarmaDownloaderFunctions.GetServiceDate(connection, serviceName, lastWorkingDateTime);
                Assert.AreEqual(date.ToString(), actualDate.ToString());

                var actualCurrentTaskId = KarmaDownloaderFunctions.GetServiceDecimal(connection, serviceName, currentTaskId);
                Assert.AreEqual(1.0m, actualCurrentTaskId);

                var actualAlias = KarmaDownloaderFunctions.GetServiceString(connection, serviceName, alias);
                Assert.AreEqual("Roman", actualAlias);

                transaction.Rollback();
            }                
        }

        [TestMethod]
        public void TestDownloadTask()
        {
            string npgConnection = GetStringConnection();
            string _serviceName = "BushuevService";
            string currentTaskId = "CURRENT_TASK_ID";
            string attemptions = "ATTEMPTIONS";

            ITaskActions taskActions = new TaskActions(npgConnection);
            IServiceActions serviceActions = new ServiceActions(npgConnection);

            using (IDbConnection connection = new NpgsqlConnection(npgConnection))
            {
                KarmaDownloaderFunctions.CreateService(connection, _serviceName);
            }

            var services = serviceActions.GetKarmaServices();

            //проверим, что у сервиса есть
            if(services.FirstOrDefault(z=>z.ServiceTitle == _serviceName) == null)
            {
                SetMessage($"Сервис:[{_serviceName}] не найден");
                return;
            }

            //проверим, что сервис рабочий
            if (services.First(z=>z.ServiceTitle == _serviceName).ServiceStatus != ServiceStatuses.Running)
            {
                SetMessage($"Сервис:[{_serviceName}] имеет статус {services.First(z => z.ServiceTitle == _serviceName).ServiceStatus.ToDbAttribute()}");
                return;
            }


            decimal? value = serviceActions.GetNumber(_serviceName, currentTaskId);
            //если работа есть, то проверили попытку выполнить данную работу
            if (value.HasValue && value != -1.0m)
            {
                long taskId = long.Parse(value.Value.ToString());
                //увеличить attemptions 
                decimal? attemption = taskActions.GetNumber(taskId, attemptions); 

                if(!attemption.HasValue)
                {
                    taskActions.SetAttribute(taskId, attemptions, 1.0m);
                }
                else
                {
                    taskActions.SetAttribute(taskId, attemptions, attemption.Value + 1);
                }

                //если кол-во > 3 то берем другую задачу
                attemption = taskActions.GetNumber(taskId, attemptions);

                if (attemption > 3.0m)
                {
                    serviceActions.SetAttribute(_serviceName, currentTaskId, -1.0m);
                    value = null;
                    //задачу поставить в статус выполнена 
                    taskActions.ErrorJob(taskId);
                }
            }

            long? taskId = null;
            KarmaDownloadJob karmaDownloadJob = null;
            //получили все работы
            if (!value.HasValue || (value.HasValue && value == -1.0m))
            {
                var tasks = taskActions.GetKarmaDownloadJob();

                if (tasks.Count(z => z.TaskStatuses == TaskStatuses.Created) != 0)
                {
                    SetMessage($"Кол-во найденных работ: {tasks.Count(z => z.TaskStatuses == TaskStatuses.Created)}");
                    foreach (var task in tasks.Where(z => z.TaskStatuses == TaskStatuses.Created))
                    {
                        var result = taskActions.RunJob(task.TaskId);
                        karmaDownloadJob = task;
                        if (result == 1)
                        {
                            taskId = task.TaskId;
                            serviceActions.SetAttribute(_serviceName, currentTaskId, 1.0m);
                            taskActions.SetAttribute(task.TaskId, attemptions, 1.0m);
                        }
                    }
                }
            }

            if(!taskId.HasValue)
            {
                return;
            }

            SetMessage($"Сервис:{_serviceName} начал работу над {taskId}");

            
            //выполнили работу 

            //изменили статус работы в 
        }

        private void SetMessage(string message, string postfix ="")
        {
            Console.WriteLine($"[{postfix}]:{message}");
        }
    }
}
