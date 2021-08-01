using AutoMapper;
using DownloaderProvider.DatabaseEntities;
using DownloaderProvider.DbFunctions;
using KarmaCore.Entities;
using KarmaCore.Interfaces;
using System.Data;

namespace DownloaderProvider
{
    public class FinInstrumentRepository : IFinInstrumentRepository
    {
        IMapper _mapper;

        public FinInstrumentRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public FinInstrument CreateOrGet(IDbConnection connection, FinInstrument finInstrument)
        {
            return _mapper.Map<DbFinInstrument, FinInstrument>(KarmaSaverFunctions.CreateOrGet(connection, _mapper.Map<FinInstrument, DbFinInstrument>(finInstrument)));
        }
    }
}
