using AutoMapper;
using Murzik.Entities.Moex;
using Murzik.Entities.MoexNew;
using Murzik.Interfaces;
using Murzik.MoexProvider.XmlEntities;
using Murzik.Utils;
using Murzik.Utils.ParseXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Murzik.MoexProvider
{
    public class MoexDownloader : IMoexDownloader
    {
        private readonly IMapper _mapper;

        public MoexDownloader(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRow(DateTime date)
        {
            var page = 0;
            var size = 100;
            long? total = null;
            var moexDatas = new List<BondDataRow>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var start = page * size;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/bonds/securities.xml?date={date.ToString("yyyy-MM-dd")}&start={start}";
                    var content = await client.GetStringAsync(url);
                    var documentXml = content.DeserializeFromXml<DocumentXml>();
                    var document = _mapper.Map<Entities.MoexNew.Document>(documentXml);

                    if (total == null)
                        total = document.HistoryCursorData.Rows.First().Total;

                    

                    if (document.HistoryData.Rows.Any())
                    {
                        moexDatas.AddRange(document.HistoryData.Rows);
                    }

                    page++;
                    if (page * size > total)
                    {
                        return moexDatas;
                    }
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<IReadOnlyCollection<MoexShareDataRow>> DownloadShareDataRow(DateTime date)
        {
            var page = 0;
            var size = 100;
            int? total = null;
            var moexDatas = new List<MoexShareDataRow>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var start = page * size;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/shares/securities.xml?date={date.ToString("yyyy-MM-dd")}&start={start}";
                    var content = await client.GetStringAsync(url);
                    var xmlDocument = XDocument.Parse(content);
                    var result = ParseXmlStructure.GetDocument(xmlDocument);
                    if (total == null)
                    {
                        var moexMetaData = result.Datas.Last().Rows.Rowss.Last() as MoexMetaRow;
                        total = moexMetaData.Total;
                    }

                    if (result.Datas.First().Rows?.Rowss?.OfType<MoexShareDataRow>()?.Count() > 0)
                    {
                        moexDatas.AddRange(result.Datas.First().Rows.Rowss.OfType<MoexShareDataRow>());
                    }

                    page++;
                    if (page * size > total)
                    {
                        return moexDatas;
                    }
                    await Task.Delay(1000);
                }
            }
        }
    }
}
