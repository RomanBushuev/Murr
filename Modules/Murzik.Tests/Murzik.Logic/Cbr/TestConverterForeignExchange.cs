using Murzik.Entities.Cbr;
using Murzik.Interfaces;
using Murzik.Logic.Cbr;
using System;
using System.Linq;
using Xunit;

namespace Murzik.Tests.Murzik.Logic.Cbr
{
    public class TestConverterForeignExchange
    {
        private IConverter _converter;
        private DateTime _constDate = new DateTime(2021, 11, 07);

        public TestConverterForeignExchange()
        {
            _converter = new ConverterForeignExchange();
        }

        [Fact]
        public void TestConvertToPackValues()
        {
            var pack = new PackCurrencies()
            {
                ValidDate = _constDate,
                Currencies = new Currencies()
                {
                    ValidDate = _constDate,
                    ValuteCursOnDates = new []
                    {
                        new ValuteCursOnDate
                        {
                            VchCode = "USD",
                            Vcode = 840,
                            VCurs = 74.1656m,
                            VName = "Доллар США          ",
                            VNom = 1
                        }
                    }
                }
            };
            var result = _converter.ConvertToPackValues(pack);
            Assert.Single(result.FinInstruments);
            var finInstrument = result.FinInstruments.First();
            Assert.Equal("USD/RUB", finInstrument.FinIdent);
            Assert.Equal(74.1656m, finInstrument.FinTimeSerieses.First().Value);
            Assert.Equal("Доллар США", finInstrument.FinStringValues.First().Value);
        }
    }
}
