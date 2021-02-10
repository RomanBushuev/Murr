using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KarmaCore.Calculations
{
    public class DownloadMoexInstruments :Calculation
    {
        public const string RunDateTime = "RunDateTime";
        public const string TypeInstrument = "TypeInstrument";

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexInstruments;

        public override void Run()
        {
            Notify("Задача на загрузку акций из MOEX начата");
            var text = Task.Run(() => GetValues()).Result;
            Console.WriteLine(text);
            Notify("Задача на загрузку акций из MOEX закончена");
        }

        private async Task<string> GetValues()
        {
            string text = string.Empty;
            using (var client = new HttpClient())
            {
                for (int i = 0; i < 10 ; i++)
                {
                    var dateTime = _paramDescriptors.ConvertDate(RunDateTime);
                    var typeInstrument = _paramDescriptors.ConvertStr(TypeInstrument);
                    var start = i * 100;
                    var url = $"https://iss.moex.com/iss/history/engines/stock/markets/{typeInstrument}/securities.xml?date={dateTime.ToString("yyyy-MM-dd")}&start={start}";
                    var content = await client.GetStringAsync(url);
                    text += content;
                    await Task.Delay(1000);
                }
            }

            return text;
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
                    Ident = TypeInstrument,
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
