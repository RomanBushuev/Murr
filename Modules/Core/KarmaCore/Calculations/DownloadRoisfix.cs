using CbrServices;
using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KarmaCore.Calculations
{
    public class DownloadRoisfix :  Calculation, IXmlResult
    {
        private XmlDocument _xmlDocument = null;
        public const string RunDateTime = "RunDateTime";

        public XmlDocument Document { get { return _xmlDocument; } set { _xmlDocument = value; } }
        public override TaskTypes TaskTypes => TaskTypes.DownloadRoisFixCbrf;

        public override void Run()
        {
            Notify($"Задача загрузки roisfix");
            DailyInfoSoap client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
            DateTime dateTime = _paramDescriptors.ConvertDate(RunDateTime);
            var temp = client.ROISfixXMLAsync(dateTime, dateTime);
            XmlDocument xmlDocument = new XmlDocument();
            var xmlNode = temp.Result;
            xmlDocument.LoadXml($"<RoisFix><ValidDate>{dateTime.ToString("yyyy-MM-dd")}</ValidDate>{xmlNode.InnerXml}</RoisFix>");
            _xmlDocument = xmlDocument;
            Notify($"Задача загрузки roisfix закончена");
        }

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
            {
                return _paramDescriptors;
            }
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
