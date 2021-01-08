using Dapper;
using Npgsql;
using NpgsqlTypes;
using ScheduleProvider.Mappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace ScheduleProvider.DbFunctions
{
    public static class KarmaSchedulerFunctions
    {
        static KarmaSchedulerFunctions()
        {
            FluentMappingInitialize.Initialize();
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


        public static long CreateCbrForeignExchangeDownload(IDbConnection dbConnection,
            CbrForeignParam param)
        {
            string function = "murr_downloader.add_cbr_foreign_exchange";

            return dbConnection.Query<long>(function,
                new { in_datetime = param.DateTime },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        public static long CreateCbrMosprimeDownload(IDbConnection dbConnection,
            CbrForeignParam param)
        {
            string function = "murr_downloader.add_cbr_mosprime";

            return dbConnection.Query<long>(function,
                new { in_datetime = param.DateTime },
                commandType: CommandType.StoredProcedure)
                .First();
        }

        /// <summary>
        /// Получаем задачи-процедуры
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static IEnumerable<ProcedureTask> GetProcedureTasks(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_procedure_tasks";

            return dbConnection.Query<ProcedureTask>(function,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Добавляем процедуру
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="procedureTask"></param>
        /// <returns></returns>
        public static long InsertProcedureTask(IDbConnection dbConnection,
            ProcedureTask procedureTask)
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
            ProcedureTask procedureTask)
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

        public static IEnumerable<ProcedureInfo> GetProcedureInfos(IDbConnection dbConnection)
        {
            string function = "murr_downloader.get_inner_procedures";

            return dbConnection.Query<ProcedureInfo>(function,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Запускаем функцию
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="function"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool RunFunction(IDbConnection dbConnection,
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
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
