using Murzik.CsvProvider;
using Murzik.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Murzik.Tests.Murzik.CsvReaderProvider
{
    public class TestCsvReaderAgent
    {
        private ICsvReaderAgent _csvReaderAgent;

        private readonly string _root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        private Dictionary<string, string> _files;

        public TestCsvReaderAgent()
        {
            _csvReaderAgent = new CsvReaderAgent();
            _files = new Dictionary<string, string>()
            {
                ["BondDescription"] = Path.Combine(_root, "Files", "Moex", "securities_stock_bonds_all.csv"),
                ["Bond"] = Path.Combine(_root, "Files", "Moex", "bond_quotes.csv"),
                ["Share"] = Path.Combine(_root, "Files", "Moex", "securities_stock_shares_all.csv"),
            };
        }

        [Fact]
        public async void TestReadBondDescriptionAsync()
        {
            var result = await _csvReaderAgent.ReadBondDescriptionAsync(_files["BondDescription"]);
            Assert.Single(result);
            var element = result.First();
            Assert.Equal("RU000A0JPN21", element.SecId);
            Assert.Equal("2 ипотечный агент АИЖК об.кл.А", element.Name);
            Assert.Equal("4-01-65388-H", element.RegNumber);
            Assert.Equal("RU000A0JPN21", element.Isin);
            Assert.Equal(new DateTime(2008, 02, 27), element.IssueDate);
            Assert.Equal(new DateTime(2040, 03, 15), element.MaturityDate);
            Assert.Equal(1000m, element.InitialFaceValue);
            Assert.Equal("SUR", element.FaceUnit);
            Assert.Equal("2-nd IA AIZhK - A", element.LatName);
            Assert.Equal(6488m, element.DaysToRedemption);
            Assert.Equal(9440000m, element.IssueSize);
            Assert.Equal(67.02m, element.FaceValue);
            Assert.Equal(4, element.CouponFrequency);
            Assert.Equal("corporate_bond", element.Type);
            Assert.Equal("Корпоративная облигация", element.TypeName);
        }

        [Fact]
        public async void TestReadBondAsync()
        {
            var result = await _csvReaderAgent.ReadBondDataRawAsync(_files["Bond"]);
            Assert.Single(result);
            var element = result.First();
            Assert.Equal("TQCB", element.BoardId);
            Assert.Equal(new DateTime(2022, 05, 27), element.TradeDate);
            Assert.Equal("UBANK11/22", element.Shortname);
            Assert.Equal(decimal.Zero, element.NumTrades);
            Assert.Equal(decimal.Zero, element.Value);
            Assert.Equal(100.3m, element.LegalClosePrice);
            Assert.Equal(decimal.Zero, element.Volume);
            Assert.Equal(101.8091m, element.MarketPrice3);
            Assert.Equal(100.3m, element.AdmittedQuote);
            Assert.Equal(decimal.Zero, element.MP2ValTrd);
            Assert.Equal(519946.91m, element.MarketPrice3TradesValue);
            Assert.Equal(decimal.Zero, element.AdmittedValue);
            Assert.Equal(new DateTime(2022, 11, 15), element.MatDate);
            Assert.Equal(5.25m, element.CouponPercent);
            Assert.Equal(1.31m, element.CouponValue);
            Assert.Equal(100m, element.FaceValue);
            Assert.Equal("SUR", element.CurrencyId);
            Assert.Equal("USD", element.FaceUnit);
            Assert.Equal(3.0m, element.TradingSession);
        }

        [Fact]
        public async void TestReadShare()
        {
            var result = await _csvReaderAgent.ReadShareDescriptions(_files["Share"]);
            Assert.Single(result);
            var element = result.First();
            Assert.Equal("ABBN", element.SecId);
            Assert.Equal("АО \"Банк Астаны\"", element.Name);
            Assert.Equal("БанкАстаны", element.ShortName);
            Assert.Equal("KZ1C00001023", element.Isin);
            Assert.Equal(36081627, element.IssueSize);
            Assert.Equal(1000, element.FaceValue);
            Assert.Equal("KZT", element.FaceUnit);
            Assert.Equal(new DateTime(2017, 12, 5), element.IssueDate);
            Assert.Equal("Bank of Astana", element.LatName);
            Assert.Equal(3, element.ListLevel);
            Assert.False(element.IsQualifiedInvestors);
            Assert.Equal("common_share", element.Type);
            Assert.Equal("Акция обыкновенная", element.TypeName);
        }
    }
}
