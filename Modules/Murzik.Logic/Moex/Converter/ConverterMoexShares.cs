using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Entities.MoexNew.Share;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Murzik.Logic.Moex.Converter
{
    public class ConverterMoexShares : IConverter
    {
        private DateTime _validDate;
        private long _moex = 2;

        public PackValues ConvertToPackValues<T>(T importData)
        {
            var shares = importData as PackMoexShares;
            _validDate = shares.ValidDate;
            return ConverterToFinInstruments(shares);
        }

        private PackValues ConverterToFinInstruments(PackMoexShares shares)
        {
            var list = new List<FinInstrument>();
            var packValues = new PackValues()
            {
                FinInstruments = list
            };

            var instruments = new HashSet<string>();
            foreach (var instrument in shares.Shares.Where(z => !string.IsNullOrEmpty(z.SecId)))
            {
                if (instruments.Contains(instrument.SecId))
                    continue;

                var finInstrument = new FinInstrument()
                {
                    FinIdent = instrument.SecId,
                    DataSourceId = _moex,
                    FinStringValues = ConvertToStringValues(instrument),
                    FinDateValues = ConvertToDateValues(instrument),
                    FinNumericValues = ConvertToNumericValues(instrument)
                };
                list.Add(finInstrument);
            }

            return packValues;
        }

        private IReadOnlyCollection<FinNumericValue> ConvertToNumericValues(ShareDescription instrument)
        {
            var finNumericValues = new List<FinNumericValue>();
            //if (instrument.IssueSize.HasValue)
            //{
            //    finNumericValues.Add(new FinNumericValue
            //    {
            //        FinAttriubte = FinAttributes.IssueSizeN.ToDbAttribute(),
            //        FromDate = _validDate,
            //        Value = instrument.IssueSize.Value
            //    });
            //}
            return finNumericValues;
        }

        private IReadOnlyCollection<FinDateValue> ConvertToDateValues(ShareDescription instrument)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyCollection<FinStringValue> ConvertToStringValues(ShareDescription instrument)
        {
            throw new NotImplementedException();
        }
    }
}
