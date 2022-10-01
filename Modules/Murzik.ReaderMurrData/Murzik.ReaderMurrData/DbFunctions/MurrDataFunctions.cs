using Dapper;
using Murzik.ReaderMurrData.DbEntities;
using Murzik.ReaderMurrData.Mappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.ReaderMurrData.DbFunctions
{
    public static class MurrDataFunctions
    {
        static MurrDataFunctions()
        {
            MurrDataMapping.Initialize();
        }

        public static async Task<IReadOnlyCollection<DbFinDataSources>> DownloadFinDataSources(IDbConnection dbConnection)
        {
            var sql = "select fin_data_source_id, fin_data_source_ident from murr_data.fin_data_sources";
            return (await dbConnection.QueryAsync<DbFinDataSources>(sql)).AsList();
        }
    }
}
