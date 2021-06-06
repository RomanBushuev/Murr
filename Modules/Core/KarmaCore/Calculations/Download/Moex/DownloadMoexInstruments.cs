using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace KarmaCore.Calculations
{
    public class DownloadMoexInstruments :Calculation, IXmlResult
    {
        private XmlDocument _xmlDocument = null;
        public const string RunDateTime = "RunDateTime";
        public const string InstrumentType = "InstrumentType";

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexInstruments;

        public XmlDocument Document { get { return _xmlDocument; } set { _xmlDocument = value; } }

        public override void Run()
        {
            Notify("Задача на загрузку акций из MOEX начата");
            var typeInstrument = _paramDescriptors.ConvertStr(InstrumentType);
            if(typeInstrument == "shares")
            {
                DownloadMoexShare();
                Notify("Задача на загрузку акций из MOEX закончена");
            }
            if(typeInstrument == "bonds")
            {
                DownloadMoexBonds();
                Notify("Задача на загрузку облигаций из MOEX закончена");
            }            
        }

        private void DownloadMoexBonds()
        {
            var text = Task.Run(() => GetValues()).Result.OfType<MoexBondDataRow>().ToList();
            List<string> strings = new List<string>(text.Count);
            foreach (var val in text)
            {
                strings.Add(ParseXmlStructure.ParseToXml(val));
            }

            string fullXml = string.Concat(strings);

            XmlDocument xmlDocument = new XmlDocument();
            var dateTime = _paramDescriptors.ConvertDate(RunDateTime);
            var typeInstrument = _paramDescriptors.ConvertStr(InstrumentType);
            xmlDocument.LoadXml($"<{typeInstrument}><ValidDate>{dateTime.ToString("yyyy-MM-dd")}</ValidDate><rows>{fullXml}</rows></{typeInstrument}>");
            _xmlDocument = xmlDocument;

        }

        private void DownloadMoexShare()
        {
            var text = Task.Run(() => GetValues()).Result.OfType<MoexShareDataRow>().ToList() ;
            List<string> strings = new List<string>(text.Count);
            foreach (var val in text)
            {
                strings.Add(ParseXmlStructure.ParseToXml(val));
            }

            string fullXml = string.Concat(strings);

            XmlDocument xmlDocument = new XmlDocument();
            var dateTime = _paramDescriptors.ConvertDate(RunDateTime);
            var typeInstrument = _paramDescriptors.ConvertStr(InstrumentType);
            xmlDocument.LoadXml($"<{typeInstrument}><ValidDate>{dateTime.ToString("yyyy-MM-dd")}</ValidDate><rows>{fullXml}</rows></{typeInstrument}>");
            _xmlDocument = xmlDocument;

        }
        private async Task<List<Row>> GetValues()
        {
            List<Row> moexDatas = new List<Row>();
            string text = string.Empty;
            int page = 0;
            int size = 100;
            int? total = null;
            using (var client = new HttpClient())
            {
                var dateTime = _paramDescriptors.ConvertDate(RunDateTime);
                var typeInstrument = _paramDescriptors.ConvertStr(InstrumentType);
                while (true)
                {
                    var start = page * size;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/{typeInstrument}/securities.xml?date={dateTime.ToString("yyyy-MM-dd")}&start={start}";
                    var content = await client.GetStringAsync(url);
                    var xmlDocument = XDocument.Parse(content);
                    var result = ParseXmlStructure.GetDocument(xmlDocument);
                    if (total == null)
                    {
                        var moexMetaData = result.Datas.Last().Rows.Rowss.Last() as MoexMetaRow;
                        total = moexMetaData.Total;
                    }

                    if (result.Datas.First().Rows.Rowss.OfType<MoexShareDataRow>().Count() != 0)
                    {
                        moexDatas.AddRange(result.Datas.First().Rows.Rowss.OfType<MoexShareDataRow>());
                    }

                    if(result.Datas.First().Rows.Rowss.OfType<MoexBondDataRow>().Count() != 0)
                    {
                        moexDatas.AddRange(result.Datas.First().Rows.Rowss.OfType<MoexBondDataRow>());
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

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
                return _paramDescriptors;
            else
            {
                _paramDescriptors = new List<ParamDescriptor>();
                _paramDescriptors.Add(new ParamDescriptor()
                { 
                    Ident = RunDateTime,
                    Description ="Время для запуска",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
                });

                _paramDescriptors.Add(new ParamDescriptor()
                { 
                    Ident = InstrumentType,
                    Description = "Тип инструмента для загрузку: index, shares, bonds, ndm, otc, ccp, deposit, repo, qnv, mamc, foreignshares, foreignndm, moexboard, gcc, credit, standard, classica",
                    ParamType = ParamType.String,
                    Value = "shares"
                });

                return _paramDescriptors;
            }
        }

        public override CalculationJson Serialize()
        {
            CalculationJson calculationJson = new CalculationJson();
            calculationJson.TaskType = (long)TaskTypes;
            calculationJson.JsonParameters = _paramDescriptors.SerializeJson();
            return calculationJson;
        }
    }
}
