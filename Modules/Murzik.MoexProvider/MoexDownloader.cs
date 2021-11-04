using AutoMapper;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using Murzik.Interfaces;
using Murzik.MoexProvider.XmlEntities.Bond;
using Murzik.MoexProvider.XmlEntities.Share;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Murzik.MoexProvider
{
    public class MoexDownloader : IMoexDownloader
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public MoexDownloader(IMapper mapper,
            ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;            
        }

        public async Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRow(DateTime date)
        {
            var page = 0;
            var size = 100;
            long? total = null;
            var moexData = new List<BondDataRow>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var start = page * size;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/bonds/securities.xml?date={date.ToString("yyyy-MM-dd")}&start={start}";
                    _logger.Info($"Отправка запроса {url}");
                    var content = await client.GetStringAsync(url);
                    var documentXml = content.DeserializeFromXml<BondDocumentXml>();
                    var document = _mapper.Map<BondDocument>(documentXml);

                    if (total == null)
                        total = document.HistoryCursorData.Rows.First().Total;                    

                    if (document.HistoryData.Rows.Any())
                        moexData.AddRange(document.HistoryData.Rows);

                    page++;
                    if (page * size > total)
                    {
                        _logger.Info($"Закончили отправлять запросы на {url}. Кол-во данных: {moexData.Count()}. Кол-во Total: {total}");
                        return moexData;
                    }
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<IReadOnlyCollection<ShareDataRow>> DownloadShareDataRow(DateTime date)
        {
            var page = 0;
            var size = 100;
            long? total = null;
            var moexData = new List<ShareDataRow>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var start = page * size;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/shares/securities.xml?date={date.ToString("yyyy-MM-dd")}&start={start}";
                    var content = await client.GetStringAsync(url);
                    var docuemntXml = content.DeserializeFromXml<ShareDocumentXml>();
                    var document = _mapper.Map<ShareDocument>(docuemntXml);

                    if (total == null)
                        total = document.HistoryCursorData.Rows.First().Total;

                    if (document.HistoryData.Rows.Any())
                        moexData.AddRange(document.HistoryData.Rows);

                    page++;
                    if(page * size > total)
                    {
                        _logger.Info($"Закончили отправлять запросы на {url}. Кол-во данных: {moexData.Count()}. Кол-во Total: {total}");
                        return moexData;
                    }

                    await Task.Delay(1000);
                }
            }
        }
    }
}
