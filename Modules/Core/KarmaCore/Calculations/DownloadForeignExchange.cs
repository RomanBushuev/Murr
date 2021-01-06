using CbrServices;
using KarmaCore;
using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace KarmaCore.Calculations
{
    public class DownloadForeignExchange : Calculation, IXmlResult
    {
        private XmlDocument _xmlDocument = null;
        public const string RunDateTime = "RunDateTime";
        public XmlDocument Document { get { return _xmlDocument; } set { _xmlDocument = value; } }
        public override TaskTypes TaskTypes => TaskTypes.DownloadCurrenciesCbrf;

        //XmlDocument IResult<XmlDocument>.Document { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Run()
        {
            Notify($"Задача загрузки иностранных валют начата");
            DailyInfoSoapClient client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            DateTime dateTime = _paramDescriptors.ConvertDate(RunDateTime);
            var temp = client.GetCursOnDateXMLAsync(dateTime);
            XmlDocument xmlDocument = new XmlDocument();
            var xmlNode = temp.Result;
            xmlDocument.LoadXml($"<Currencies><ValidDate>{dateTime.ToString("dd.MM.yyyy")}</ValidDate>{xmlNode.InnerXml}</Currencies>");
            _xmlDocument = xmlDocument;
            Notify($"Задача загрузки иностранных валют закончена");
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
                    Description = "Время для запуска",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
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
