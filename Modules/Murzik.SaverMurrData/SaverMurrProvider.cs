using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.SaverMurrData.DbEntities;
using Murzik.SaverMurrData.DbFunctions;
using NLog;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Murzik.SaverMurrData
{
    public class SaverMurrProvider : ISaverMurrData
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _connection;

        public SaverMurrProvider(ILogger logger,
            IMapper mapper,
            string connection)
        {
            _logger = logger;
            _mapper = mapper;
            _connection = connection;
        }

        public void Save(PackValues packValues)
        {
            using (var connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using (var tranasction = connection.BeginTransaction())
                {
                    SaveFinInstruments(connection, packValues.FinInstruments);
                    SaveFinDateValues(connection, packValues.FinInstruments);
                    SaveFinNumericValues(connection, packValues.FinInstruments);
                    SaveFinStringValues(connection, packValues.FinInstruments);
                    SaveFinTimeSeries(connection, packValues.FinInstruments);
                    tranasction.Commit();
                }
            }
        }

        public void SaveFinInstruments(IDbConnection connection,
            IReadOnlyCollection<FinInstrument> FinInstruments)
        {
            _logger.Info($"Количество финансовых инструментов для всатвки: {FinInstruments.Count}");
            foreach (var finInstrument in FinInstruments)
            {
                var value = KarmaSaverFunctions.InsertFinInstrument(connection, _mapper.Map<DbFinInstrument>(finInstrument));
                finInstrument.FinId = value;
            }
        }

        private void SaveFinDateValues(IDbConnection connection,
            IReadOnlyCollection<FinInstrument> finInstruments)
        {
            var values = finInstruments.Select(z => _mapper.Map<IReadOnlyCollection<DbFinDataValue>>(z)).SelectMany(z => z);
            _logger.Info($"Количество дат для финансовых инструментов для всатвки: {values.Count()}");
            foreach (var finDataValue in values)
                KarmaSaverFunctions.InsertFinDataValue(connection, finDataValue);
        }

        private void SaveFinStringValues(IDbConnection connection,
            IReadOnlyCollection<FinInstrument> finInstruments)
        {
            var values = finInstruments.Select(z => _mapper.Map<IReadOnlyCollection<DbFinStringValue>>(z)).SelectMany(z => z);
            _logger.Info($"Количество строк для финансовых инструментов для всатвки: {values.Count()}");
            foreach (var finDataValue in values)
                KarmaSaverFunctions.InsertFinStringValue(connection, finDataValue);
        }

        private void SaveFinNumericValues(IDbConnection connection,
            IReadOnlyCollection<FinInstrument> finInstruments)
        {
            var values = finInstruments.Select(z => _mapper.Map<IReadOnlyCollection<DbFinNumericValue>>(z)).SelectMany(z => z);
            _logger.Info($"Количество чисел для финансовых инструментов для всатвки: {values.Count()}");
            foreach (var finDataValue in values)
                KarmaSaverFunctions.InsertFinNumericValue(connection, finDataValue);
        }

        private void SaveFinTimeSeries(IDbConnection connection,
            IReadOnlyCollection<FinInstrument> finInstruments)
        {
            var values = finInstruments.Select(z => _mapper.Map<IReadOnlyCollection<DbFinTimeSeries>>(z)).SelectMany(z => z);
            _logger.Info($"Количество временных серий для финансовых инструментов для всатвки: {values.Count()}");
            foreach (var finDataValue in values)
                KarmaSaverFunctions.InsertTimeSeries(connection, finDataValue);
        }
    }
}
