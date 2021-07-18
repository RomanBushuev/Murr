using KarmaCore.Entities;
using KarmaCore.Interfaces;
using KarmaCore.Utils;
using Npgsql;
using System.Linq;

namespace DownloaderProvider
{
    public class MarkerRepository : IMarkerRepository
    {
        private string _connection;
        private string _rub = "RUB";
        private IFinInstrumentRepository _finInstrumentRepository;
        private IFinDataSourceRepository _finDataSourceRepository;

        public MarkerRepository(string connection, 
            IFinInstrumentRepository finInstrumentRepository,
            IFinDataSourceRepository finDataSourceRepository)
        {
            _connection = connection;
            _finInstrumentRepository = finInstrumentRepository;
            _finDataSourceRepository = finDataSourceRepository;
        }

        public void Save(Currencies currencies)
        {
            using (var conn = new NpgsqlConnection(_connection))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var dataSourceId = _finDataSourceRepository.GetFinDataSources(conn).First(z => z.FinDataSourceIdent == "CBR").FinDataSourceId;

                    foreach (var currency in currencies.ValuteCursOnDates)
                    {
                        var finInstrument = new FinInstrument()
                        {
                            Ident = $"{currency.VchCode}/{_rub}",
                            DataSourceId = dataSourceId
                        };

                        finInstrument = _finInstrumentRepository.CreateOrGet(conn, finInstrument);
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
