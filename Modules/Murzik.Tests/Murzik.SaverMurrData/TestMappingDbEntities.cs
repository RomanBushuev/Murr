using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.SaverMurrData;
using Murzik.SaverMurrData.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Murzik.Tests.Murzik.SaverMurrData
{
    public class TestMappingDbEntities
    {
        private IMapper _mapper;
        private DateTime constDate = new DateTime(2021, 11, 05);

        public TestMappingDbEntities()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
        }

        [Fact]
        public void TestFinInstrumentToDbFinInstrument()
        {
            var finInstrument = new FinInstrument()
            {
                FinId = 1,
                FinIdent = "isin",
                DataSourceId = 2,
                FinDateValues = new[]
                {
                    new FinDateValue { FinAttriubte = "MATURITY", FromDate =  constDate, Value = constDate}
                },
                FinNumericValues = new[]
                {
                    new FinNumericValue { FinAttriubte = "NOMINAL", FromDate = constDate, Value = 100.0m }
                },
                FinStringValues = new[]
                {
                    new FinStringValue { FinAttriubte = "ISIN", FromDate = constDate, Value = "RU3214123" }
                },
                FinTimeSerieses = new[]
                {
                    new FinTimeSeries { FinAttriubte = "CLOSE", Date = constDate, Value = 132.0m }
                }
            };
            var db = _mapper.Map<DbFinInstrument>(finInstrument);
            Assert.Equal("isin", db.FinInstrumentIdent);
            Assert.Equal(2, db.DataSourceId);
        }

        [Fact]
        public void TestFinInstrumentToDbFinDataValue()
        {
            var finInstrument = new FinInstrument()
            {
                FinId = 1,
                FinIdent = "isin",
                DataSourceId = 2,
                FinDateValues = new[]
                {
                    new FinDateValue { FinAttriubte = "MATURITY", FromDate =  constDate, Value = constDate}
                },
                FinNumericValues = new[]
                {
                    new FinNumericValue { FinAttriubte = "NOMINAL", FromDate = constDate, Value = 100.0m }
                },
                FinStringValues = new[]
                {
                    new FinStringValue { FinAttriubte = "ISIN", FromDate = constDate, Value = "RU3214123" }
                },
                FinTimeSerieses = new[]
                {
                    new FinTimeSeries { FinAttriubte = "CLOSE", Date = constDate, Value = 132.0m }
                }
            };
            var db = _mapper.Map<IReadOnlyCollection<DbFinDataValue>>(finInstrument).First();
            Assert.Equal("MATURITY", db.FinAttributeIdent);
            Assert.Equal(constDate, db.FromDate);
            Assert.Equal(constDate, db.Value);
            Assert.Equal(1, db.FinInstrumentId);
        }

        [Fact]
        public void TestFinInstrumentToDbFinNumericValue()
        {

        }

        [Fact]
        public void TestFinInstrumentToDbFinStringValue()
        {

        }

        [Fact]
        public void TestFinInstrumentToDbFinTimeSeries()
        {

        }
    }
}
