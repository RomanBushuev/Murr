using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.ReaderMurrData.DbFunctions;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.ReaderMurrData
{
    public class ReaderMurrProvider : IReaderMurrProvider
    {
        //private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _connection;

        public ReaderMurrProvider(
            //ILogger logger,
            IMapper mapper,
            string connection)
        {
            //_logger = logger;
            _mapper = mapper;
            _connection = connection;
        }

        public async Task<IReadOnlyCollection<FinDataSources>> GetFinDataSources()
        {
            using (var connection = new NpgsqlConnection(_connection))
            {
                var values = await MurrDataFunctions.DownloadFinDataSources(connection);
                return values.Select(z => _mapper.Map<FinDataSources>(z)).ToList();
            }
        }
    }
}
