using KarmaCore.BaseTypes;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using System.Collections.Generic;

namespace KarmaCore.Calculations
{
    public class CalculationFactory
    {
        public static Calculation GetCalculation(CalculationJson json)
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

            return null;
        }
    }
}
