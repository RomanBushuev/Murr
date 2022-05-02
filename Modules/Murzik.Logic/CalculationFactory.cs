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
            IAlgorithm calculation = null;
            switch (json.TaskType)
            {
                case (long)TaskTypes.DownloadCurrenciesCbrf:
                    {
                        calculation = new DownloadForeignExchange(_logger,
                            _taskAction,
                            _cbrDownloader,
                            _xmlSave);
                        break;
                    }
                case (long)TaskTypes.DownloadMosPrimeCbrf:
                    {
                        calculation = new DownloadMosprime(_logger,
                            _taskAction,
                            _cbrDownloader,
                            _xmlSave);
                        break;
                    }
                case (long)TaskTypes.DownloadKeyRateCbrf:
                    {
                        calculation = new DownloadKeyRate(_logger,
                            _taskAction,
                            _cbrDownloader,
                            _xmlSave);
                        break;
                    }
                case (long)TaskTypes.DownloadRoisFixCbrf:
                    {
                        calculation = new DownloadRoisfix(_logger,
                            _taskAction,
                            _cbrDownloader,
                            _xmlSave);
                        break;
                    }
                case (long)TaskTypes.DownloadRuoniaCbrf:
                    {
                        calculation = new DownloadRuonia(_logger,
                            _taskAction,
                            _cbrDownloader,
                            _xmlSave);
                        break;
                    }
                case (long)TaskTypes.DownloadMoexInstruments:
                    {
                        calculation = new DownloadMoexInstruments(_logger,
                            _taskAction,
                            _moexDownloader,
                            _xmlSave,
                            _csvSaver);
                        break;
                    }
                case (long)TaskTypes.SaveForeignExchange:
                    {
                        calculation = new SaveForeignExchange(_logger,
                            _taskAction,
                            _convertFactory,
                            _saverMurrData,
                            _cbrDownloader);
                        break;
                    }
                case (long)TaskTypes.DownloadMoexCoupons:
                    {
                        calculation = new DownloadMoexCoupons(_logger,
                            _taskAction,
                            _moexDownloader,
                            _csvSaver);
                        break;
                    }
                case (long)TaskTypes.DownloadMoexAmortizations:
                    {
                        calculation = new DownloadMoexAmortizations(_logger,
                            _taskAction,
                            _moexDownloader,
                            _csvSaver);
                        break;
                    }
                case (long)TaskTypes.DownloadMoexOffers:
                    {
                        calculation = new DownloadMoexOffers(_logger,
                            _taskAction,
                            _moexDownloader,
                            _csvSaver);
                        break;
                    }
                default:
                    {
                        calculation = null;
                        break;
                    }
            }

            var values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
            foreach (var val in values)
                calculation.SetParamDescriptors(val);

            return calculation;
        }
    }
}
