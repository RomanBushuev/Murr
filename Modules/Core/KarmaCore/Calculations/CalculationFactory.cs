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
            if(json.TaskType == (long)TaskTypes.DownloadCurrenciesCbrf)
            {
                Calculation calculation = new DownloadForeignExchange();
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
