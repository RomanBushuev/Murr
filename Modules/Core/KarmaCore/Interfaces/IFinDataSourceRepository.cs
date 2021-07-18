using KarmaCore.Enumerations;
using System.Collections.Generic;
using System.Data;

namespace KarmaCore.Interfaces
{
    public interface IFinDataSourceRepository
    {
        public IReadOnlyCollection<FinDataSource> GetFinDataSources(IDbConnection connection);
    }
}
