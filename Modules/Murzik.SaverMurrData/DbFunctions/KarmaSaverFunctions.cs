using Dapper;
using Murzik.SaverMurrData.DbEntities;
using System.Data;

namespace Murzik.SaverMurrData.DbFunctions
{
    public static class KarmaSaverFunctions
    {
        public static long InsertFinInstrument(IDbConnection dbConnection,
            DbFinInstrument dbFinInstrument)
        {
            string function = "murr_data.insert_fin_instrument";

            return dbConnection.QueryFirst<long>(function,
                new
                {
                    in_data_source_id = dbFinInstrument.DataSourceId,
                    in_fin_ident = dbFinInstrument.FinInstrumentIdent,
                },
                commandType: CommandType.StoredProcedure);
        }

        public static void InsertFinDataValue(IDbConnection dbConnection,
            DbFinDataValue dbFinDataValue)
        {
            string function = "murr_data.insert_fin_date_value";

            dbConnection.Execute(function,
                new
                {
                    in_fin_id = dbFinDataValue.FinInstrumentId,
                    in_fin_attribute = dbFinDataValue.FinAttributeIdent,
                    in_value = dbFinDataValue.Value,
                    in_date_from = dbFinDataValue.FromDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public static void InsertFinNumericValue(IDbConnection dbConnection,
            DbFinNumericValue dbFinNumericValue)
        {
            string function = "murr_data.insert_fin_numeric_value";

            dbConnection.Execute(function,
                new
                {
                    in_fin_id = dbFinNumericValue.FinInstrumentId,
                    in_fin_attribute = dbFinNumericValue.FinAttributeIdent,
                    in_value = dbFinNumericValue.Value,
                    in_date_from = dbFinNumericValue.FromDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public static void InsertFinStringValue(IDbConnection dbConnection, 
            DbFinStringValue dbFinStringValue)
        {
            string function = "murr_data.insert_fin_string_value";

            dbConnection.Execute(function,
                new
                {
                    in_fin_id = dbFinStringValue.FinInstrumentId,
                    in_fin_attribute = dbFinStringValue.FinAttributeIdent,
                    in_value = dbFinStringValue.Value,
                    in_date_from = dbFinStringValue.FromDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public static void InsertTimeSeries(IDbConnection dbConnection,
            DbFinTimeSeries dbFinTimeSeries)
        {
            string function = "murr_data.insert_time_series";

            dbConnection.Execute(function,
                new
                {
                    in_fin_id = dbFinTimeSeries.FinInstrumentId,
                    in_fin_attribute = dbFinTimeSeries.FinAttributeIdent,
                    in_value = dbFinTimeSeries.Value,
                    in_date_from = dbFinTimeSeries.Date
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
