using Dapper;
using DownloaderProvider.DatabaseEntities;
using System.Collections.Generic;
using System.Data;

namespace DownloaderProvider.DbFunctions
{
    public static class KarmaSaverFunctions
    {
        static KarmaSaverFunctions()
        {
            FluentMappingInitialize.Initialize();
        }

        /// <summary>
        /// Источники
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<DbFinDataSource> GetAll(IDbConnection connection)
        {
            var function = "murr_data.get_data_sources";
            return connection.Query<DbFinDataSource>(function,
                commandType: CommandType.StoredProcedure).AsList();
        }

        /// <summary>
        /// Инструменты
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="finInstrument"></param>
        /// <returns></returns>
        public static DbFinInstrument CreateOrGet (IDbConnection connection, DbFinInstrument finInstrument)
        {
            var function = "murr_data.insert_fin_instrument";
            var id = connection.QueryFirst<long>(function,
                new
                {
                    in_data_source_id = finInstrument.DataSourceId,
                    in_fin_ident = finInstrument.Ident
                },
                commandType: CommandType.StoredProcedure);
            finInstrument.FinInstrumentId = id;
            return finInstrument;
        }
    }
}
