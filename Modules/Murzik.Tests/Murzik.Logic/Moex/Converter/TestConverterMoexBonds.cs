using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Interfaces;
using Murzik.Logic.Moex.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Murzik.Tests.Murzik.Logic.Moex
{
    public class TestConverterMoexBonds
    {
        private IConverter _converter;
        private DateTime _constDate = new DateTime(2022, 06, 12);

        public TestConverterMoexBonds()
        {
            _converter = new ConverterMoexBonds();
        }

        [Fact]
        public void TestConvertToPackValues()
        {
            var pack = new PackMoexBonds()
            {
                ValidDate = _constDate,
                Bonds = new[]
                {
                    new BondDescription
                    {
                        SecId = "RU000A0JPN21",
                        Name = "2 ипотечный агент АИЖК об.кл.А",
                        RegNumber = "4-01-65388-H",
                        Isin = "RU000A0JPN21",
                        IssueDate = new DateTime(2008, 02, 27),
                        MaturityDate = new DateTime(2040, 03, 15),
                        InitialFaceValue = 1000m,
                        FaceUnit = "SUR",
                        LatName = "2-nd IA AIZhK - A",
                        DaysToRedemption = 6488m,
                        IssueSize = 9440000m,
                        FaceValue = 67.02m,
                        CouponFrequency = 4,
                        Type = "corporate_bond",
                        TypeName = "Корпоративная облигация"
                    }
                }
            };

            var result = _converter.ConvertToPackValues(pack);
            Assert.Single(result.FinInstruments);
            var element = result.FinInstruments.First();
            Assert.Equal("RU000A0JPN21", element.FinIdent);
            Assert.Single(element.FinNumericValues);
            Assert.Equal(2, element.FinDateValues.Count);
            Assert.Equal(3, element.FinStringValues.Count);
        }
    }
}
