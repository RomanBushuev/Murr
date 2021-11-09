using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Logic.Cbr;
using Murzik.Logic.Moex;
using Murzik.Utils;
using NLog;
using System.Collections.Generic;

namespace Murzik.Logic
{
    public class CalculationFactory : ICalculationFactory
    {
        private ILogger _logger;
        private ITaskActions _taskAction;
        private ICbrDownloader _cbrDownloader;
        private IMoexDownloader _moexDownloader;
        private IXmlSaver _xmlSave;
        private ICsvSaver _csvSaver;
        private IConverterFactory _convertFactory;
        private ISaverMurrData _saverMurrData;

        public CalculationFactory(ILogger logger,
            ITaskActions taskAction,
            ICbrDownloader cbrDownloader,
            IMoexDownloader moexDownloader,
            IXmlSaver xmlSaver,
            ICsvSaver csvSaver,
            IConverterFactory converterFactory,
            ISaverMurrData saverMurrData)
        {
            _logger = logger;
            _taskAction = taskAction;
            _cbrDownloader = cbrDownloader;
            _moexDownloader = moexDownloader;
            _xmlSave = xmlSaver;
            _csvSaver = csvSaver;
            _convertFactory = converterFactory;
            _saverMurrData = saverMurrData;
        }

        public IAlgorithm GetCalculation(CalculationJson json)
        {
            if (json.TaskType == (long)TaskTypes.DownloadCurrenciesCbrf)
            {
                var calculation = new DownloadForeignExchange(_logger,
                    _taskAction,
                    _cbrDownloader,
                    _xmlSave);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadMosPrimeCbrf)
            {
                var calculation = new DownloadMosprime(_logger,
                    _taskAction,
                    _cbrDownloader,
                    _xmlSave);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadKeyRateCbrf)
            {
                var calculation = new DownloadKeyRate(_logger,
                    _taskAction,
                    _cbrDownloader,
                    _xmlSave);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadRoisFixCbrf)
            {
                var calculation = new DownloadRoisfix(_logger,
                    _taskAction,
                    _cbrDownloader,
                    _xmlSave);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadRuoniaCbrf)
            {
                var calculation = new DownloadRuonia(_logger,
                    _taskAction,
                    _cbrDownloader,
                    _xmlSave);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadMoexInstruments)
            {
                var calculation = new DownloadMoexInstruments(_logger,
                    _taskAction,
                    _moexDownloader,
                    _xmlSave,
                    _csvSaver);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.SaveForeignExchange)
            {
                var calculation = new SaveForeignExchange(_logger,
                    _taskAction, 
                    _convertFactory,
                    _saverMurrData,
                    _cbrDownloader);
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            return null;
        }
    }
}
