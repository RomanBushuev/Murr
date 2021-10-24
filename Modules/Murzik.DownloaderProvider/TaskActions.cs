using AutoMapper;
using Murzik.DownloaderProvider.DbFunctions;
using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Murzik.DownloaderProvider
{
    public class TaskActions : ITaskActions
    {
        private readonly string _connection;
        private readonly IMapper _mapper;

        public TaskActions(string connection,
            IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public long RunJob(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
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
                return KarmaDownloaderFunctions.ChangeTaskStatus(connection,
                    taskId,
                    (long)TaskStatuses.Running,
                    (long)TaskStatuses.Error);
            }
        }

        public IReadOnlyCollection<KarmaDownloadJob> GetKarmaDownloadJob()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                return KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection).Select(z => _mapper.Map<KarmaDownloadJob>(z)).ToList();
            }
        }

        public void SetAttribute(long taskId, string attribute, string text)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
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
            using (IDbConnection connection = new NpgsqlConnection(_connection))
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
                return _mapper.Map<CalculationJson>(KarmaDownloaderFunctions.GetTaskTemplates(connection, taskTemplateId));
            }
        }

        public SaverJson GetSaverJson(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                //взять 
                var job = KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection).FirstOrDefault(z => z.TaskId == taskId);
                if(job is not null && job.SaverTemplateId.HasValue)
                    return _mapper.Map<SaverJson>(KarmaDownloaderFunctions.GetSaverTemplates(connection, job.SaverTemplateId.Value));
                return null;
            }
        }

        public bool IsAlive(long taskId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connection))
            {
                var job = _mapper.Map<KarmaDownloadJob>(KarmaDownloaderFunctions.DownloadKarmaDownloadJobs(connection).FirstOrDefault(z => z.TaskId == taskId));
                return job.TaskStatuses == TaskStatuses.Running;                    
            }
        }
    }
}
