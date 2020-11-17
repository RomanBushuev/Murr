using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
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
using DownloaderProvider.Entities;
using System.Text.Json;

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
                    KarmaDownloaderFunctions.InsertNumeric(connection, taskId, attemptions, 2);
                    KarmaDownloaderFunctions.InsertTaskDate(connection, taskId, startTask, dateTime);
                    KarmaDownloaderFunctions.InsertTaskDateText(connection, taskId, log, dateTime, "Hello");
                    transaction.Rollback();
                }
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


                    transaction.Commit();
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
    }
}
