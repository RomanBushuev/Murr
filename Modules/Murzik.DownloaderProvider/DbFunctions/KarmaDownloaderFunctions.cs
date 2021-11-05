using Dapper;
using Murzik.DownloaderProvider.DbEntities;
using Murzik.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Murzik.DownloaderProvider.DbFunctions
{
    public static class KarmaDownloaderFunctions
    {
        static KarmaDownloaderFunctions()
        {
            KarmaDownloaderMapping.Initialize();
        }

        internal class JsonParameter : ICustomQueryParameter
        {
            private readonly string _value;

            public JsonParameter(string value)
            {
                _value = value;
            }

            public void AddParameter(IDbCommand command, string name)
            {
                var parameter = new NpgsqlParameter(name, NpgsqlDbType.Jsonb);
                parameter.Value = _value;

                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Получаем все задачи
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static IEnumerable<DbKarmaDownloadJob> DownloadKarmaDownloadJobs(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_jobs";

            return dbConnection.Query<DbKarmaDownloadJob>(function,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Изменяем статус задачи
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="oldTaskStatus"></param>
        /// <param name="newTaskStatus"></param>
        /// <returns></returns>
        public static long ChangeTaskStatus(IDbConnection dbConnection,
            long taskId,
            long oldTaskStatus,
            long newTaskStatus)
        {
            string function = "murr_downloader.change_task_status";

            return dbConnection.QueryFirst<long>(function,
                new
                {
                    in_task_id = taskId,
                    in_old_task_status = oldTaskStatus,
                    in_new_task_status = newTaskStatus
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавим значения даты к задаче по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <param name="dateTime"></param>
        public static void InsertTaskDate(IDbConnection dbConnection,
            long taskId,
            string taskAttribute,
            DateTime dateTime)
        {
            string function = "murr_downloader.insert_task_date";

            dbConnection.Execute(function,
                new
                {
                    in_task_id = taskId,
                    in_task_attribute = taskAttribute,
                    in_task_value = dateTime
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавим значение даты и сообщения к задаче по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <param name="dateTime"></param>
        /// <param name="message"></param>
        public static void InsertTaskDateText(IDbConnection dbConnection,
            long taskId,
            string taskAttribute,
            DateTime dateTime,
            string message)
        {
            string function = "murr_downloader.insert_task_date_string";

            dbConnection.Execute(function,
                new
                {
                    in_task_id = taskId,
                    in_task_attribute = taskAttribute,
                    in_task_date = dateTime,
                    in_task_value = message
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавим значение числа к задаче по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <param name="taskValue"></param>
        public static void InsertTaskNumeric(IDbConnection dbConnection,
            long taskId,
            string taskAttribute,
            decimal taskValue)
        {
            string function = "murr_downloader.insert_task_numeric";

            dbConnection.Execute(function,
                new
                {
                    in_task_id = taskId,
                    in_task_attribute = taskAttribute,
                    in_task_value = taskValue
                },
                commandType: CommandType.StoredProcedure);
        }

        public static long CreateService(IDbConnection dbConnection,
            string serviceName,
            string serviceVersion)
        {
            string function = "murr_downloader.add_service";

            return dbConnection.QueryFirst<long>(function,
                new
                {
                    in_service_name = serviceName,
                    in_service_version = serviceVersion
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавим значение строки к задаче по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <param name="taskValue"></param>
        public static void InsertTaskString(IDbConnection dbConnection,
            long taskId,
            string taskAttribute,
            string taskValue)
        {
            string function = "murr_downloader.insert_task_string";

            dbConnection.Execute(function,
                new
                {
                    in_task_id = taskId,
                    in_task_attribute = taskAttribute,
                    in_task_value = taskValue
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем значение даты к сервису по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <param name="serviceValue"></param>
        public static void InsertServiceDate(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute,
            DateTime serviceValue)
        {
            string function = "murr_downloader.insert_service_date";

            dbConnection.Execute(function,
                new
                {
                    in_service_name = serviceName,
                    in_service_attribute = serviceAttribute,
                    in_service_value = serviceValue
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Изменения шаблона
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="saverTempalateId"></param>
        /// <param name="saverJson"></param>
        internal static void UpdateSaverTemplates(IDbConnection dbConnection, long saverTempalateId, SaverJson saverJson)
        {
            string function = "murr_downloader.update_saver_template";

            dbConnection.Execute(function,
                new
                {
                    in_saver_template_id = saverTempalateId,
                    in_saver_json = saverJson.JsonParameters
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем значение строки к сервису по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <param name="serviceValue"></param>
        public static void InsertServiceString(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute,
            string serviceValue)
        {
            string function = "murr_downloader.insert_service_string";

            dbConnection.Execute(function,
                new
                {
                    in_service_name = serviceName,
                    in_service_attribute = serviceAttribute,
                    in_service_value = serviceValue
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем значение числа к сервису по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <param name="serviceValue"></param>
        public static void InsertServiceNumeric(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute,
            decimal serviceValue)
        {
            string function = "murr_downloader.insert_service_numeric";

            dbConnection.Execute(function,
                new
                {
                    in_service_name = serviceName,
                    in_service_attribute = serviceAttribute,
                    in_service_value = serviceValue
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем значение числа к сервису по атрибуту 
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <param name="serviceDateTime"></param>
        /// <param name="serviceValue"></param>
        public static void InsertServiceDateString(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute,
            DateTime serviceDateTime,
            string serviceValue)
        {
            string function = "murr_downloader.insert_service_date_string";

            dbConnection.Execute(function,
                new
                {
                    in_service_name = serviceName,
                    in_service_attribute = serviceAttribute,
                    in_service_date = serviceDateTime,
                    in_service_value = serviceValue
                },
                commandType: CommandType.StoredProcedure);
        }

        //Получение значение задачи по атрибуту 
        /// <summary>
        /// Получаем строку у задачи
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <returns></returns>
        public static string GetTaskString(IDbConnection dbConnection,
            long taskId,
            string taskAttribute)
        {
            string function = "murr_downloader.get_task_string";

            return dbConnection.QueryFirst<string>(function,
                new
                {
                    in_task_id = taskId,
                    in_attribute = taskAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Получаем число у задачи
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <returns></returns>
        public static decimal? GetTaskDecimal(IDbConnection dbConnection,
            long taskId,
            string taskAttribute)
        {
            string function = "murr_downloader.get_task_numeric";

            return dbConnection.QueryFirst<decimal?>(function,
                new
                {
                    in_task_id = taskId,
                    in_attribute = taskAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Получаем дату у задачи
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="taskId"></param>
        /// <param name="taskAttribute"></param>
        /// <returns></returns>
        public static DateTime? GetTaskDate(IDbConnection dbConnection,
            long taskId,
            string taskAttribute)
        {
            string function = "murr_downloader.get_task_date";

            return dbConnection.QueryFirst<DateTime?>(function,
                new
                {
                    in_task_id = taskId,
                    in_attribute = taskAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        //Получение значение сервиса по атрибуту 
        /// <summary>
        /// Получаем строку у сервиса
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <returns></returns>
        public static string GetServiceString(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute)
        {
            string function = "murr_downloader.get_service_string";

            return dbConnection.QueryFirst<string>(function,
                new
                {
                    in_service_name = serviceName,
                    in_attribute = serviceAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Получаем число сервиса по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <returns></returns>
        public static decimal? GetServiceDecimal(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute)
        {
            string function = "murr_downloader.get_service_numeric";

            return dbConnection.QueryFirst<decimal?>(function,
                new
                {
                    in_service_name = serviceName,
                    in_attribute = serviceAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Получаем значение даты сервиса по атрибуту
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceAttribute"></param>
        /// <returns></returns>
        public static DateTime? GetServiceDate(IDbConnection dbConnection,
            string serviceName,
            string serviceAttribute)
        {
            string function = "murr_downloader.get_service_date";

            return dbConnection.QueryFirst<DateTime?>(function,
                new
                {
                    in_service_name = serviceName,
                    in_attribute = serviceAttribute
                },
                commandType: CommandType.StoredProcedure);
        }

        public static IEnumerable<DbKarmaService> DownloadKarmaServices(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_services";

            return dbConnection.Query<DbKarmaService>(function,
                commandType: CommandType.StoredProcedure);
        }

        public static DbCalculationJson GetTaskTemplates(IDbConnection dbConnection,
            long taskTemplateId)
        {
            string function = "murr_downloader.get_task_template";

            return dbConnection.QueryFirstOrDefault<DbCalculationJson>(function,
                new
                {
                    in_task_template_id = taskTemplateId,
                },
                commandType: CommandType.StoredProcedure);
        }

        public static DbSaverJson GetSaverTemplates(IDbConnection dbConnection,
            long saverTemplateId)
        {
            string function = "murr_downloader.get_saver_template";

            return dbConnection.QueryFirstOrDefault<DbSaverJson>(function,
                new
                {
                    in_saver_template_id = saverTemplateId
                },
                commandType: CommandType.StoredProcedure);
        }

        public static void ChangeServiceStatus(IDbConnection dbConnection,
            string serviceName, 
            long serviceStatusId)
        {
            string function = "murr_downloader.change_service_status";

            dbConnection.Execute(function,
                new
                {
                    in_service_title = serviceName,
                    in_new_service_status_id = serviceStatusId
                },
                commandType: CommandType.StoredProcedure);
        }

        public static async Task SetHealthCheckAsync(IDbConnection dbConnection,
             long serviceId,
             DateTime dateTime)
        {
            string function = "murr_downloader.update_health_check";

            await dbConnection.ExecuteAsync(function,
                new
                {
                    in_service_id = serviceId,
                    in_service_time = dateTime
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
