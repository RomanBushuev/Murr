using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using Murzik.Entities;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.SaverMurrData;
using NLog;
using System;
using Xunit;

namespace Murzik.Tests.Murzik.SaverMurrData
{
    public class TestSaverMurrProvider
    {
        private Mock<ILogger> _logger;
        private ISaverMurrData _saverMurrData;
        private DateTime constDate = new DateTime(2021, 11, 05);

        

        public TestSaverMurrProvider()
        {
            IMapper mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _logger = new Mock<ILogger>();

            var dataProvider = TestExtensions.GetConfiguration()
                .GetSection("DataProvider")
                .Get<DataProvider>();

            _saverMurrData = new SaverMurrProvider(_logger.Object, mapper, dataProvider.KarmaSaver);
        }

        [Fact]
        public void TestSave()
        {
            var pack = new PackValues()
            {
                FinInstruments = new[]
                {
                    new FinInstrument()
                    {
                        FinId = 1,
                        FinIdent = "ISIN",
                        DataSourceId = 0,
                        FinDateValues = new[]
                        {
                            new FinDateValue { FinAttriubte = "MATURITY_DATE", FromDate =  constDate, Value = constDate}
                        },
                        FinNumericValues = new[]
                        {
                            new FinNumericValue { FinAttriubte = "FACEVALUE", FromDate = constDate, Value = 100.0m }
                        },
                        FinStringValues = new[]
                        {
                            new FinStringValue { FinAttriubte = "ISIN", FromDate = constDate, Value = "RU3214123" }
                        },
                        FinTimeSerieses = new[]
                        {
                            new FinTimeSeries { FinAttriubte = "CLOSE", Date = constDate, Value = 132.0m }
                        }
                    }
                }
            };
            _saverMurrData.Save(pack);
        }
    }
}
