using CbrSecuritiesInfo;
using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CbrSecuritiesInfo;

namespace KarmaCore.Calculations
{
    public class DownloadG2 : Calculation, IXmlResult
    {
        private XmlDocument _xmlDocument = null;
        public const string RunDateTime = "RunDateTime";
        public XmlDocument Document { get { return _xmlDocument; } set { _xmlDocument = value; } }
        public override TaskTypes TaskTypes => TaskTypes.DownloadG2CurveCbrf;

        public override void Run()
        {
            Notify($"Задача загрузки кривой G2 из ЦБ начата");
            var client = new SecInfoSoapClient(SecInfoSoapClient.EndpointConfiguration.SecInfoSoap);
            DateTime dateTime = _paramDescriptors.ConvertDate(RunDateTime);
            var temp = client.GCurveAsync(dateTime);
            XmlDocument xmlDocument = new XmlDocument();
            var xmlNode = temp.Result;
            xmlDocument.LoadXml($"<G2><ValidDate>{dateTime.ToString("yyyy-MM-dd")}</ValidDate>{xmlNode}</G2>");
            _xmlDocument = xmlDocument;
            Notify($"Задача загрузки кривой G2 из ЦБ закончена");
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
