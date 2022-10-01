using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Murzik.Logic.Moex.Converter
{
    public class ConverterMoexBonds : IConverter
    {
        private DateTime _validDate;
        private long _moex = 2;

        public PackValues ConvertToPackValues<T>(T importData)
        {
            var bonds = importData as PackMoexBonds;
            _validDate = bonds.ValidDate;
            return ConvertToFinInstrument(bonds);
        }

        private PackValues ConvertToFinInstrument(PackMoexBonds bonds)
        {
            var list = new List<FinInstrument>();
            var packValues = new PackValues()
            {
                FinInstruments = list
            };

            var instruments = new HashSet<string>();
            foreach (var instrument in bonds.Bonds.Where(z => !string.IsNullOrEmpty(z.SecId)))
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

        private IReadOnlyCollection<FinNumericValue> ConvertToNumericValues(BondDescription instrument)
        {
            var finNumericValues = new List<FinNumericValue>();
            if (instrument.IssueSize.HasValue)
            {
                finNumericValues.Add(new FinNumericValue
                {
                    FinAttriubte = FinAttributes.IssueSizeN.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.IssueSize.Value
                });
            }
            return finNumericValues;
        }

        private IReadOnlyCollection<FinDateValue> ConvertToDateValues(BondDescription instrument)
        {
            var finDateValues = new List<FinDateValue>();
            if (instrument.IssueDate.HasValue)
                finDateValues.Add(new FinDateValue
                {
                    FinAttriubte = FinAttributes.IssueDateD.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.IssueDate.Value
                });

            if (instrument.MaturityDate.HasValue)
                finDateValues.Add(new FinDateValue
                {
                    FinAttriubte = FinAttributes.MaturityDateD.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.MaturityDate.Value
                });

            return finDateValues;
        }

        private IReadOnlyCollection<FinStringValue> ConvertToStringValues(BondDescription instrument)
        {
            var finStringValues = new List<FinStringValue>();
            if (!string.IsNullOrEmpty(instrument.Isin))
                finStringValues.Add(new FinStringValue
                {
                    FinAttriubte = FinAttributes.IsinS.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.Isin
                });
            if (!string.IsNullOrEmpty(instrument.TypeName))
                finStringValues.Add(new FinStringValue
                {
                    FinAttriubte = FinAttributes.TypeS.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.TypeName
                });
            if (!string.IsNullOrEmpty(instrument.RegNumber))
                finStringValues.Add(new FinStringValue
                {
                    FinAttriubte = FinAttributes.Regnumber.ToDbAttribute(),
                    FromDate = _validDate,
                    Value = instrument.RegNumber
                });

            return finStringValues;            
        }
    }
}
