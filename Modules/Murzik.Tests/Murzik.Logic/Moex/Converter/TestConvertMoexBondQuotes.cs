using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Interfaces;
using Murzik.Logic.Moex.Converter;
using Murzik.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Murzik.Tests.Murzik.Logic.Moex.Converter
{
    public class TestConvertMoexBondQuotes
    {
        private IConverter _converter;
        private DateTime _constDate = new DateTime(2022, 06, 10);

        public TestConvertMoexBondQuotes()
        {
            _converter = new ConverterMoexBondQuotes();
        }

        [Fact]
        public void TestConvertToPackValues()
        {
            var pack = new PackMoexBondQuote()
            {
                QuoteSources = new List<string>() { "TQCB", "TQOB" },
                Bonds = new List<BondDataRow>()
                {
                    new BondDataRow
                    {
                        Secid = "RU000A0JPN21",
                        BoardId = "TQCB",
                        NumTrades = 1,
                        Value = 2,
                        Low = 3,
                        High = 4,
                        Close = 5,
                        Waprice = 6,
                        Open = 7,
                        Volume = 8,
                    }
                }
            };
            var result = _converter.ConvertToPackValues(pack);
            Assert.Single(result.FinInstruments);
            var element = result.FinInstruments.First();
            Assert.Equal(8, element.FinTimeSerieses.Count);
            Assert.Equal(1, element.FinTimeSerieses.First(z=>z.FinAttriubte == FinAttributes.NumtradesT.ToDbAttribute()).Value);
            Assert.Equal(8, element.FinTimeSerieses.First(z => z.FinAttriubte == FinAttributes.VolumeT.ToDbAttribute()).Value);
        }
    }
}
