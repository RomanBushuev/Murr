using Dapper;
using DownloaderProvider.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Data;

namespace DownloaderProvider.DbFunctions
{
    public static class KarmaDownloaderFunctions
    {
        static KarmaDownloaderFunctions()
        {
            FluentMappingInitialize.Initialize();
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
            string function = "murr_downloader.get_task";

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
        public static void InsertNumeric(IDbConnection dbConnection,
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
    }
}
