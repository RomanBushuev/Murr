using AutoMapper;
using DownloaderProvider.DatabaseEntities;
using DownloaderProvider.DbFunctions;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DownloaderProvider
{
    public class FinDataSourceRepository : IFinDataSourceRepository
    {
        IMapper _mapper;

        public FinDataSourceRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IReadOnlyCollection<FinDataSource> GetFinDataSources(IDbConnection connection)
        {
            return KarmaSaverFunctions.GetAll(connection).Select(z => _mapper.Map<DbFinDataSource, FinDataSource>(z)).ToList();
        }
    }
}