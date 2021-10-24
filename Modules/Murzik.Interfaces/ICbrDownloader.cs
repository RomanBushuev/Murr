using System;
using System.Threading.Tasks;
using System.Xml;

namespace Murzik.Interfaces
{
    public interface ICbrDownloader
    {
        public Task<XmlDocument> DownloadForeignExchange(DateTime date);

        public Task<XmlDocument> DownloadG2(DateTime date);

        public Task<XmlDocument> DownloadKeyRate(DateTime date);

        public Task<XmlDocument> DownloadMosPrime(DateTime date);

        public Task<XmlDocument> DownloadRoisfix(DateTime date);

        public Task<XmlDocument> DownloadRuonia(DateTime date);
    }
}
