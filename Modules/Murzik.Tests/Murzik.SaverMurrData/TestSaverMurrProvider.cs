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

        public static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return config;
        }

        public TestSaverMurrProvider()
        {
            IMapper mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _logger = new Mock<ILogger>();

            var dataProvider = GetConfiguration()
                .GetSection("DataProvider")
                .Get<DataProvider>();

            _saverMurrData = new SaverMurrProvider(_logger.Object, mapper, dataProvider.KarmaAdmin);
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
                        FinDataValues = new[]
                        {
                            new FinDataValue { FinAttriubte = "MATURITY_DATE", FromDate =  constDate, Value = constDate}
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
