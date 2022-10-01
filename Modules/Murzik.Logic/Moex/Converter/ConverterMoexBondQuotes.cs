using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Murzik.Logic.Moex.Converter
{
    public class ConverterMoexBondQuotes : IConverter
    {
        private long _moex = 2;

        public PackValues ConvertToPackValues<T>(T importData)
        {
            var bonds = importData as PackMoexBondQuote;
            return ConvertToFinInstrument(bonds);
        }

        private PackValues ConvertToFinInstrument(PackMoexBondQuote bonds)
        {
            var list = new List<FinInstrument>();
            var packValues = new PackValues()
            {
                FinInstruments = list
            };

            var instruments = new HashSet<string>();
            foreach (var quoteSource in bonds.QuoteSources)
            {
                foreach (var instrument in bonds.Bonds.Where(z => z.BoardId == quoteSource &&
                    !string.IsNullOrEmpty(z.Secid)))
                {
                    if (instruments.Contains(instrument.Secid))
                        continue;

                    var finInstrument = new FinInstrument()
                    {
                        FinIdent = instrument.Secid,
                        DataSourceId = _moex,
                        FinTimeSerieses = ConvertToTimeSeries(instrument)
                    };
                    list.Add(finInstrument);
                }
            }

            return packValues;
        }

        private IReadOnlyCollection<FinTimeSeries> ConvertToTimeSeries(BondDataRow instrument)
        {
            var finTimeSeries = new List<FinTimeSeries>();
            if (instrument.NumTrades != decimal.Zero)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.NumtradesT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.NumTrades
                });
            if (instrument.Volume != decimal.Zero)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.VolumeT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Volume
                });
            if (instrument.Low.HasValue)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.LowT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Low.Value
                });
            if (instrument.High.HasValue)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.HighT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.High.Value
                });
            if (instrument.Close.HasValue)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.CloseT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Close.Value
                });
            if (instrument.Waprice.HasValue)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.WapriceT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Waprice.Value
                });
            if (instrument.Open.HasValue)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.OpenT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Open.Value
                });
            if (instrument.Volume != decimal.Zero)
                finTimeSeries.Add(new FinTimeSeries()
                {
                    FinAttriubte = FinAttributes.VolumeT.ToDbAttribute(),
                    Date = instrument.TradeDate,
                    Value = instrument.Volume
                });

            return finTimeSeries;
        }
    }
}
