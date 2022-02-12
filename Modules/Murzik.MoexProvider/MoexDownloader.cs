using AutoMapper;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
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
        private readonly IJsonMoexParser _jsonMoexParser;

        public MoexDownloader(IMapper mapper,
            ILogger logger,
            IJsonMoexParser jsonMoexParser)
        {
            _mapper = mapper;
            _logger = logger;
            _jsonMoexParser = jsonMoexParser;
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

        public async Task<IReadOnlyCollection<Coupon>> DownloadCoupons(long? attemptions = null)
        {
            var currentAttemptions = 100;
            var page = 0;
            var size = 100;
            long? total = null;
            var moexData = new List<Coupon>();
            while (true)
            {
                try
                {
                    while (true)
                    {
                        using (var client = new HttpClient())
                        {
                            {
                                var start = page * size;
                                var url = $"https://iss.moex.com/iss/statistics/engines/stock/markets/bonds/bondization.json?iss.only=coupons,coupons.cursor&sort_order=desc&iss.json=extended&iss.meta=off&lang=ru&limit=100&start={start}";
                                _logger.Info($"Отправка запроса {url}");
                                var content = await client.GetStringAsync(url);
                                var couponInformation = _jsonMoexParser.ConvertFromRootJson(content);

                                if (total == null)
                                    total = couponInformation.CouponCursors.First().Total;

                                if (couponInformation.Coupons.Any())
                                    moexData.AddRange(couponInformation.Coupons);

                                page++;
                                if (page * size > total)
                                {
                                    _logger.Info($"Закончили отправлять запросы на {url}. Кол-во данных: {moexData.Count()}. Кол-во Total: {total}");
                                    return moexData;
                                }

                                if (attemptions.HasValue)
                                {
                                    attemptions--;
                                    if (attemptions <= 0)
                                        return moexData;
                                }

                                await Task.Delay(1000);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _logger.Info($"Осталось повторных попыток запросить данные: {currentAttemptions}");
                    currentAttemptions--;
                    await Task.Delay(new Random().Next(1, 10) * 1000);
                    if (currentAttemptions <= 0)
                    {
                        _logger.Info($"Количество попыток на зпрос данных исчерпано, завершаем ошибкой");
                        throw new Exception("Не удалось прочесть купоны по облигациям");
                    }
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
                    _logger.Info($"Отправка запроса {url}");
                    var content = await client.GetStringAsync(url);
                    var docuemntXml = content.DeserializeFromXml<ShareDocumentXml>();
                    var document = _mapper.Map<ShareDocument>(docuemntXml);

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
    }
}
