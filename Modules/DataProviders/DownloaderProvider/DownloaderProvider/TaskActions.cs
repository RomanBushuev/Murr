using DownloaderProvider.DbFunctions;
using KarmaCore.Entities;
using KarmaCore.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DownloaderProvider.DatabaseEntities;
using KarmaCore.Enumerations;

namespace DownloaderProvider
{
    public class TaskActions : ITaskActions
    {
        private readonly string _connection;

        public TaskActions(string connection)
        {
            _connection = connection;
        }

        public long RunJob(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.ChangeTaskStatus(connection,
                    taskId,
                    (long)TaskStatuses.Created,
                    (long)TaskStatuses.Running);
            }
        }

        public long EndJob(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.ChangeTaskStatus(connection,
                    taskId,
                    (long)TaskStatuses.Running,
                    (long)TaskStatuses.Done);
            }
        }

        public long ErrorJob(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                return KarmaDownloaderFunctions.ChangeTaskStatus(connection,
                    taskId,
                    (long)TaskStatuses.Running,
                    (long)TaskStatuses.Error);
            }
        }

        public List<KarmaDownloadJob> GetKarmaDownloadJob()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                var result = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection);
                return ConverterDto.ConvertDto<DbKarmaDownloadJob, KarmaDownloadJob>(result).ToList();
            }
        }        

        public void SetAttribute(long taskId, string attribute, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertTaskString(connection,
                    taskId, 
                    attribute.Trim().ToUpper(),
                    text);                    
            }
        }

        public void SetAttribute(long taskId, string attribute, decimal number)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertTaskNumeric(connection,
                    taskId, 
                    attribute.Trim().ToUpper(), 
                    number);
            }

        }

        public void SetAttribute(long taskId, string attribute, DateTime dateTime)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertTaskDate(connection, 
                    taskId,
                    attribute.Trim().ToUpper(),
                    dateTime);
            }
        }

        public void SetAttribute(long taskId, string attribute, DateTime dateTime, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                KarmaDownloaderFunctions.InsertTaskDateText(connection, taskId,
                    attribute.Trim().ToUpper(),
                    dateTime,
                    text);
            }
        }

        public string GetString(long taskId, string attribute)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.GetTaskString(connection,
                    taskId,
                    attribute.Trim().ToUpper());
            }
        }

        public decimal? GetNumber(long taskId, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.GetTaskDecimal(connection,
                    taskId,
                    attribute.Trim().ToUpper());
            }
        }

        public DateTime? GetDate(long taskId, string attribute)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.GetTaskDate(connection,
                    taskId,
                    attribute.Trim().ToUpper());
            }
        }

        public CalculationJson GetCalculationJson(long taskTemplateId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                var t = KarmaDownloaderFunctions.GetTaskTemplates(connection, taskTemplateId);
                return ConverterDto.ConvertDto<DbCalculationJson, CalculationJson>(t);                
            }
        }

        public SaverJson GetSaverJson(long saverTemplateId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                var t = KarmaDownloaderFunctions.GetSaverTemplates(connection, saverTemplateId);
                return ConverterDto.ConvertDto<DbSaverJson, SaverJson>(t);
            }
        }
    }
}
