using Murzik.Entities.Cbr;
using Murzik.Entities.Cbr.Packs;
using Murzik.Entities.Enumerations;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.Utils;
using System;
using System.Collections.Generic;

namespace Murzik.Logic.Cbr.Converter
{
    public class ConverterForeignExchange : IConverter
    {
        private DateTime _validDate;
        private long _cbrSource = 1;

        public PackValues ConvertToPackValues<T>(T importData)
        {
            var exchanges = importData as PackCurrencies;
            _validDate = exchanges.ValidDate;
            return ConvertToFinInstruments(exchanges.Currencies);
        }

        private PackValues ConvertToFinInstruments(Currencies exchange)
        {
            var list = new List<FinInstrument>();
            var packValues = new PackValues()
            {
                FinInstruments = list
            };

            foreach (var currency in exchange.ValuteCursOnDates)
            {
                FinInstrument finInstrument = new FinInstrument()
                {
                    FinIdent = $"{currency.VchCode}/RUB",
                    DataSourceId = _cbrSource,
                    FinStringValues = ConvertToStringValues(currency),
                    FinTimeSerieses = ConvertToTimerSeries(currency)
                };
                list.Add(finInstrument);
            }

            return packValues;
        }

        private IReadOnlyCollection<FinStringValue> ConvertToStringValues(ValuteCursOnDate currency)
        {
            var finStringValues = new List<FinStringValue>();
            if (!string.IsNullOrEmpty(currency.VName.Trim()))
                finStringValues.Add(new FinStringValue
                {
                    FinAttriubte = FinAttributes.ShortNameS.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = currency.VName.Trim()
                });

            finStringValues.Add(new FinStringValue
            {
                FinAttriubte = FinAttributes.DigitalCodeS.ToDbAttribute(),
                FromDate = _validDate,
                Value = currency.Vcode.ToString()
            });

            finStringValues.Add(new FinStringValue
            {
                FinAttriubte = FinAttributes.TypeS.ToDbAttribute(),
                FromDate = _validDate,
                Value = "EXCHANGE RATE"
            });

            return finStringValues;
        }

        private IReadOnlyCollection<FinTimeSeries> ConvertToTimerSeries(ValuteCursOnDate currency)
        {
            var finTimeSeries = new List<FinTimeSeries>();
            finTimeSeries.Add(new FinTimeSeries
            {
                FinAttriubte = FinAttributes.CloseT.ToDbAttribute(),
                Date = _validDate,
                Value = currency.VCurs / currency.VNom
            });
            return finTimeSeries;
        }
    }
}
