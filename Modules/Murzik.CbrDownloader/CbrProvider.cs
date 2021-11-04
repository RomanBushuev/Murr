using CbrSecuritiesInfo;
using CbrServices;
using Murzik.Interfaces;
using NLog;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Murzik.CbrDownloader
{
    public class CbrProvider : ICbrDownloader
    {
        private readonly ILogger _logger;

        public CbrProvider(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<XmlDocument> DownloadForeignExchange(DateTime date)
        {
            _logger.Info($"Загрузка иносстранных валют");
            var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            var temp = await client.GetCursOnDateXMLAsync(date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<Currencies><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp.InnerXml}</Currencies>");
            return xmlDocument;
        }

        public async Task<XmlDocument> DownloadG2(DateTime date)
        {
            _logger.Info($"Загрузка G2");
            var client = new SecInfoSoapClient(SecInfoSoapClient.EndpointConfiguration.SecInfoSoap);
            var temp = await client.GCurveAsync(date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<G2><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp}</G2>");
            return xmlDocument;
        }

        public async Task<XmlDocument> DownloadKeyRate(DateTime date)
        {
            _logger.Info($"Загрузка ключевой ставки");
            var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            var temp = await client.KeyRateXMLAsync(date, date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<KeyRate><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp.InnerXml}</KeyRate>");
            return xmlDocument;
        }

        public async Task<XmlDocument> DownloadMosPrime(DateTime date)
        {
            _logger.Info($"Загрузка мос прайм");
            var client = new SecInfoSoapClient(SecInfoSoapClient.EndpointConfiguration.SecInfoSoap);
            var temp = await client.MosPrimeXMLAsync(date, date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<MosPrime><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp.InnerXml}</MosPrime>");
            return xmlDocument;
        }

        public async Task<XmlDocument> DownloadRoisfix(DateTime date)
        {
            _logger.Info($"Загрузка роисфикс");
            var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            var temp = await client.ROISfixXMLAsync(date, date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<RoisFix><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp.InnerXml}</RoisFix>");
            return xmlDocument;
        }

        public async Task<XmlDocument> DownloadRuonia(DateTime date)
        {
            _logger.Info($"Загрузка руони");
            var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            var temp = await client.RuoniaXMLAsync(date, date);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml($"<Ruonia><ValidDate>{date.ToString("yyyy-MM-dd")}</ValidDate>{temp.InnerXml}</Ruonia>");
            return xmlDocument;
        }
    }
}
