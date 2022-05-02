using Murzik.Entities.Cbr.Packs;
using Murzik.Interfaces;
using Murzik.Logic;
using Murzik.Logic.Cbr;
using Xunit;

namespace Murzik.Tests.Murzik.Logic
{
    public class TestConverterFactory
    {
        private IConverterFactory _converterFactory;

        public TestConverterFactory()
        {
            _converterFactory = new ConverterFactory();
            _converterFactory.AddTConverter(typeof(PackCurrencies), new ConverterForeignExchange());
        }

        [Fact]
        public void TestGetConverter()
        {
            var converter = _converterFactory.GetConverter(typeof(PackCurrencies));
            Assert.NotNull(converter);
        }
    }
}
