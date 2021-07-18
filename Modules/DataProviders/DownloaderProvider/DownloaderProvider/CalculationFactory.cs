using KarmaCore;
using KarmaCore.BaseTypes;
using KarmaCore.Calculations;
using KarmaCore.Calculations.Saver;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System.Collections.Generic;

namespace DownloaderProvider
{
    public class CalculationFactory : ICalculationFactory
    {
        private ICbrXmlRepository _cbrXmlRepository;
        private IMarkerRepository _markerRepository;
        public CalculationFactory(ICbrXmlRepository cbrXmlRepository,
            IMarkerRepository markerRepository)
        {
            _cbrXmlRepository = cbrXmlRepository;
            _markerRepository = markerRepository;
        }        

        public Calculation GetCalculation(CalculationJson json)
        {
            if (json.TaskType == (long)TaskTypes.DownloadCurrenciesCbrf)
            {
                Calculation calculation = new DownloadForeignExchange();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadMosPrimeCbrf)
            {
                Calculation calculation = new DownloadMosprime();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadKeyRateCbrf)
            {
                Calculation calculation = new DownloadKeyRate();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadRoisFixCbrf)
            {
                Calculation calculation = new DownloadRoisfix();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadRuoniaCbrf)
            {
                Calculation calculation = new DownloadRuonia();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.DownloadMoexInstruments)
            {
                Calculation calculation = new DownloadMoexInstruments();
                List<ParamDescriptor> values = ParamDescriptorExtensions.DeserializeJson(json.JsonParameters, calculation.GetParamDescriptors());
                foreach (var val in values)
                {
                    calculation.SetParamDescriptors(val);
                }
                return calculation;
            }

            if (json.TaskType == (long)TaskTypes.SaveForeignExchange)
            {
                Calculation calculation = new SaveForeignExchange(_cbrXmlRepository, _markerRepository);
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
