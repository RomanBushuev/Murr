using AutoMapper;
using Microsoft.Extensions.Options;
using Murzik.Entities.MoexNew;
using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Offer;
using Murzik.Entities.MoexNew.Share;
using Murzik.Interfaces;
using Murzik.MoexProvider.XmlEntities.Bond;
using Murzik.MoexProvider.XmlEntities.Share;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Murzik.MoexProvider
{
    public class MoexDownloader : IMoexDownloader
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IJsonMoexParser _jsonMoexParser;
        private MoexSettings _moexSettings;

        public MoexDownloader(IMapper mapper,
            ILogger logger,
            IJsonMoexParser jsonMoexParser,
            IOptions<MoexSettings> moexSettings)
        {
            _mapper = mapper;
            _logger = logger;
            _jsonMoexParser = jsonMoexParser;
            _moexSettings = moexSettings.Value;
        }

        public async Task<IReadOnlyCollection<Amortization>> DownloadAmortizationsAsync(long? amountOfPagesToDownload = null)
        {
            var currentAttemptions = 100;
            var page = 0;
            var size = 100;
            long? total = null;
            var moexData = new List<Amortization>();
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
                                var url = $"https://iss.moex.com/iss/statistics/engines/stock/markets/bonds/bondization.json?iss.only=amortizations,amortizations.cursor&sort_order=desc&iss.json=extended&iss.meta=off&lang=ru&limit=100&start={start}";
                                _logger.Info($"Отправка запроса {url}");
                                var content = await client.GetStringAsync(url);
                                var couponInformation = _jsonMoexParser.ConvertToAmortizationInformationAndGetLast(content);

                                if (total == null)
                                    total = couponInformation.AmortizationCursors.First().Total;

                                if (couponInformation.Amortizations.Any())
                                    moexData.AddRange(couponInformation.Amortizations);

                                page++;
                                if (page * size > total)
                                {
                                    _logger.Info($"Закончили отправлять запросы на {url}. Кол-во данных: {moexData.Count()}. Кол-во Total: {total}");
                                    return moexData;
                                }

                                if (amountOfPagesToDownload.HasValue)
                                {
                                    amountOfPagesToDownload--;
                                    if (amountOfPagesToDownload <= 0)
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

        public async Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRowAsync(DateTime downloadDate)
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
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/bonds/securities.xml?date={downloadDate.ToString("yyyy-MM-dd")}&start={start}";
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

        public async Task<IReadOnlyCollection<Coupon>> DownloadCouponsAsync(long? amountOfPagesToDownload = null)
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
                                var couponInformation = _jsonMoexParser.ConvertToCouponInformationAndGetLast(content);

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

                                if (amountOfPagesToDownload.HasValue)
                                {
                                    amountOfPagesToDownload--;
                                    if (amountOfPagesToDownload <= 0)
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

        public async Task<IReadOnlyCollection<Offer>> DownloadOffersAsync(long? amountOfPageToDownload = null)
        {
            var currentAttemptions = 100;
            var page = 0;
            var size = 100;
            long? total = null;
            var moexData = new List<Offer>();
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
                                var url = $"https://iss.moex.com/iss/statistics/engines/stock/markets/bonds/bondization.json?iss.only=offers,offers.cursor&sort_order=desc&iss.json=extended&iss.meta=off&lang=ru&limit=100&start={start}";
                                _logger.Info($"Отправка запроса {url}");
                                var content = await client.GetStringAsync(url);
                                var couponInformation = _jsonMoexParser.ConvertToOfferInformationAndGetLast(content);

                                if (total == null)
                                    total = couponInformation.OfferCursors.First().Total;

                                if (couponInformation.Offers.Any())
                                    moexData.AddRange(couponInformation.Offers);

                                page++;
                                if (page * size > total)
                                {
                                    _logger.Info($"Закончили отправлять запросы на {url}. Кол-во данных: {moexData.Count()}. Кол-во Total: {total}");
                                    return moexData;
                                }

                                if (amountOfPageToDownload.HasValue)
                                {
                                    amountOfPageToDownload--;
                                    if (amountOfPageToDownload <= 0)
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

        public async Task<IReadOnlyCollection<ShareDataRow>> DownloadShareDataRowAsync(DateTime downloadDate)
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
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/shares/securities.xml?date={downloadDate.ToString("yyyy-MM-dd")}&start={start}";
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

        public async Task<string> DownloadShareDescriptionAsync()
        {
            var path = Path.Combine(_moexSettings.FolderPath, "ShareDescription", $"{DateTime.Today:yyyy.MM.dd}");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var file = Path.Combine(path, $"{DateTime.Today:yyyy.MM.dd}.zip");
            _logger.Info($"Загрузка описательных данных по акциям будет произведена в: {file}");
            var url = "https://iss.moex.com/iss/downloads/securitygroups/stock_shares/collections/stock_shares_all/securities_stock_shares_all.csv.zip";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(url, file);
            ZipFile.ExtractToDirectory(file, path, true);

            var files = Directory.GetFiles(path).Where(z => z != file);
            if (files.Count() > 1)
                _logger.Warn($"Количество файлов при загрузке по {url} больше одного : {string.Join(",", files)}");
            else if (files.Count() == 1)
                File.Delete(file);

            _logger.Info($"Загрузка описательных данных по акциям произведена");
            return files.FirstOrDefault();
        }

        public async Task<string> DownloadBondDescriptionAsync()
        {
            var path = Path.Combine(_moexSettings.FolderPath, "BondDescription", $"{DateTime.Today:yyyy.MM.dd}");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var file = Path.Combine(path, $"{DateTime.Today:yyyy.MM.dd}.zip");
            _logger.Info($"Загрузка описательных данных по облигациям будет произведена в: {file}");
            var url = "https://iss.moex.com/iss/downloads/securitygroups/stock_bonds/collections/stock_bonds_all/securities_stock_bonds_all.csv.zip";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(url, file);
            ZipFile.ExtractToDirectory(file, path, true);

            var files = Directory.GetFiles(path).Where(z => z != file);
            if (files.Count() > 1)
                _logger.Warn($"Количество файлов при загрузке по {url} больше одного : {string.Join(",", files)}");
            else if (files.Count() == 1)
                File.Delete(file);

            _logger.Info($"Загрузка описательных данных по облигациям произведена");
            return files.FirstOrDefault();
        }

        public async Task<string> DownloadEurobondDescriptionAsync()
        {
            var path = Path.Combine(_moexSettings.FolderPath, "EurobondDescription", $"{DateTime.Today:yyyy.MM.dd}");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var file = Path.Combine(path, $"{DateTime.Today:yyyy.MM.dd}.zip");
            _logger.Info($"Загрузка описательных данных по облигациям будет произведена в: {file}");
            var url = "https://iss.moex.com/iss/downloads/securitygroups/stock_eurobond/collections/stock_eurobond_all/securities_stock_eurobond_all.csv.zip";
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(url, file);
            ZipFile.ExtractToDirectory(file, path, true);

            var files = Directory.GetFiles(path).Where(z => z != file);
            if (files.Count() > 1)
                _logger.Warn($"Количество файлов при загрузке по {url} больше одного : {string.Join(",", files)}");
            else if (files.Count() == 1)
                File.Delete(file);

            _logger.Info($"Загрузка описательных данных по облигациям произведена");
            return files.FirstOrDefault();
        }

    }
}
