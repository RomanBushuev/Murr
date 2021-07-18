using KarmaCore.Interfaces;
using KarmaCore.Utils;
using System.Xml.Linq;

namespace DownloaderProvider
{
    public class CbrRepository : ICbrXmlRepository
    {
        public Currencies GetCurrencies(string filename)
        {
            var xmlDocument = XDocument.Load(filename);
            var result = ParseXmlStructure.GetCurrencies(xmlDocument);
            return result;
        }
    }
}
