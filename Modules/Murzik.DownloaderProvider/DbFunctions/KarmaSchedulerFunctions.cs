using Dapper;
using Murzik.DownloaderProvider;
using Murzik.DownloaderProvider.DbEntities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Dapper.SqlMapper;

namespace Murzik.DownloaderProvider.DbFunctions
{
    public static class KarmaSchedulerFunctions
    {
        static KarmaSchedulerFunctions()
        {
            KarmaSchedulerMapping.Initialize();
        }

        public class JsonParameter : ICustomQueryParameter
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
        /// Формирование задачи для загрузки курсов
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long CreateCbrForeignExchangeDownload(IDbConnection dbConnection,
            DateTime date)
        {
            string function = "murr_downloader.add_cbr_foreign_exchange";

            return dbConnection.Query<long>(function,
                new { in_datetime = date },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        /// <summary>
        /// Формирование задачи на загрузку mosprime
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long CreateCbrMosprimeDownload(IDbConnection dbConnection,
            DateTime date)
        {
            string function = "murr_downloader.add_cbr_mosprime";

            return dbConnection.Query<long>(function,
                new { in_datetime = date },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        /// <summary>
        /// Загрузка задачи на загрузку ключевой ставки
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long CreateCbrKeyRateDownload(IDbConnection dbConnection,
            DateTime date)
        {
            string function = "murr_downloader.add_cbr_keyrate";

            return dbConnection.Query<long>(function,
                new { in_datetime = date },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        /// <summary>
        /// Загрузка задачи на загрузку руонии
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long CreateCbrRuoniaDownload(IDbConnection dbConnection,
            DateTime date)
        {
            string function = "murr_downloader.add_cbr_ruonia";

            return dbConnection.Query<long>(function,
                new { in_datetime = date },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        public static long CreateCbrRoisFixDownload(IDbConnection dbConnection,
            DateTime date)
        {
            string function = "murr_downloader.add_cbr_roisfix";

            return dbConnection.Query<long>(function,
                new { in_datetime = date },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        public static long CreateMoexShares(IDbConnection dbConnection,
            DateTime date, string instrumentType)
        {
            string function = "murr_downloader.add_moex_instruments";

            return dbConnection.Query<long>(function,
                new { in_datetime = date, in_type_instruments = instrumentType },
                commandType: CommandType.StoredProcedure)
                .First();
        }


        /// <summary>
        /// Получаем задачи-процедуры
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static IEnumerable<DbProcedureTask> GetProcedureTasks(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_procedure_tasks";

            return dbConnection.Query<DbProcedureTask>(function,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем процедуру
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="procedureTask"></param>
        /// <returns></returns>
        public static long InsertProcedureTask(IDbConnection dbConnection,
            DbProcedureTask procedureTask)
        {
            string function = "murr_downloader.insert_procedure_task";
            
            return dbConnection.Query<long>(function,
                new
                {
                    in_procedure_title = procedureTask.ProcedureTitle,
                    in_procedure_is_use = procedureTask.ProcedureIsUse,
                    in_procedure_params = new JsonParameter(procedureTask.ProcedureParams),
                    in_procedure_template = procedureTask.ProcedureTemplate,
                    in_procedure_last_run = procedureTask.ProcedureLastRun,
                    in_procedure_next_run = procedureTask.ProcedureNextRun
                },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        /// <summary>
        /// Изменяем значения
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="procedureTask"></param>
        public static void ChangeProcedureTask(IDbConnection dbConnection,
            DbProcedureTask procedureTask)
        {
            string function = "murr_downloader.change_procedure_task";

            dbConnection.Execute(function,
                new
                {
                    in_procedure_task_id = procedureTask.ProcedureTaskId,
                    in_procedure_last_run = procedureTask.ProcedureLastRun,
                    in_procedure_next_run = procedureTask.ProcedureNextRun
                },
                commandType: CommandType.StoredProcedure);
        }

        public static IEnumerable<DbProcedureInfo> GetProcedureInfos(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_inner_procedures";

            return dbConnection.Query<DbProcedureInfo>(function,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Запускаем функцию
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="function"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static (bool isExecuted, Exception exception) RunFunction(IDbConnection dbConnection,
            string function,
            Dictionary<string, object> values)
        {
            try
            {
                if(dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                NpgsqlCommand command = new NpgsqlCommand(function, (NpgsqlConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;
                foreach(var val in values)
                {
                    command.Parameters.AddWithValue(val.Key, val.Value);
                }                
                command.ExecuteNonQuery();
                return (true, null);
            }
            catch(Exception ex)
            {
                return (false, ex);
            }
        }
    }
}
