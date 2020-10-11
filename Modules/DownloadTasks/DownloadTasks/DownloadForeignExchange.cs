using CbrServices;
using KarmaCore;
using KarmaCore.BaseTypes;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace DownloadTasks
{
    public class DownloadForeignExchange : Calculation, IXmlResult
    {
        private XmlDocument _xmlDocument = null;
        public const string RunDateTime = "RunDateTime";
        public XmlDocument Document => _xmlDocument;

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
            _paramDescriptors.Clear();
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
}
