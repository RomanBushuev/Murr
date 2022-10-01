using AutoMapper;
using Murzik.CbrDownloader;
using Murzik.CbrDownloader.XmlEntities;
using Murzik.Entities.Cbr;
using Murzik.Utils;
using System;
using System.IO;
using Xunit;

namespace Murzik.Tests.Murzik.CbrDownloader
{
    public class TestMappingXmlEntities
    {
        private readonly IMapper _mapper;
        private readonly string _root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public TestMappingXmlEntities()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
        }

        [Fact]
        public void TestKeyRateXml()
        {
            var document = new KeyRate
            {
                ValidDate = new DateTime(2021, 10, 03),
                Kr = new Kr
                { 
                    Dt = new DateTime(2021, 10, 04),
                    Rate = 6.25m
                }
            };

            var documentXml = _mapper.Map<KeyRateXml>(document);
            var xml = documentXml.SerializeToClearXml();
            var newDocument = xml.DeserializeFromXml<KeyRateXml>();

            Assert.Equal(new DateTime(2021, 10, 03), newDocument.ValidDate);
            Assert.Equal(new DateTime(2021, 10, 04), newDocument.Kr.Dt);
            Assert.Equal(6.25m, newDocument.Kr.Rate);
        }

        [Fact]
        public void TestKeyRateXmlFile()
        {
            string moexDocument = @"\Files\Cbr\CbrKeyrate.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<KeyRateXml>();

            Assert.Equal(new DateTime(2021, 10, 12), newDocument.ValidDate);
            Assert.Equal(6.75m, newDocument.Kr.Rate);
        }

        [Fact]
        public void TestCurrencies()
        {
            var document = new Currencies
            {
                ValidDate = new DateTime(2021, 10, 03),
                ValuteCursOnDates = new[]
                {
                    new ValuteCursOnDate
                    {
                        VchCode = "AUD",
                        Vcode = 36,
                        VName = "Австралийский доллар",
                        VCurs = 58.4860m,
                        VNom = 1
                    },
                    new ValuteCursOnDate
                    {
                        VchCode = "AZN",
                        Vcode = 944,
                        VName = "Азербайджанский манат",
                        VCurs = 44.4694m,
                        VNom = 1
                    },
                }
            };

            var documentXml = _mapper.Map<CurrenciesXml>(document);
            var xml = documentXml.SerializeToClearXml();
            var newDocument = xml.DeserializeFromXml<CurrenciesXml>();

            Assert.Equal(new DateTime(2021, 10, 03), newDocument.ValidDate);

            Assert.Equal("AUD", newDocument.ValuteCursOnDates[0].VchCode);
            Assert.Equal(36, newDocument.ValuteCursOnDates[0].Vcode);
            Assert.Equal("Австралийский доллар", newDocument.ValuteCursOnDates[0].VName.Trim());
            Assert.Equal(58.4860m, newDocument.ValuteCursOnDates[0].VCurs);
            Assert.Equal(1, newDocument.ValuteCursOnDates[0].VNom);

        }

        [Fact]
        public void TestCurrenciesXmlFile()
        {
            string moexDocument = @"\Files\Cbr\CbrForeignExchange.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<CurrenciesXml>();

            Assert.Equal(new DateTime(2021, 07, 17), newDocument.ValidDate);
        }

        [Fact]
        public void TestMosPrime()
        {
            var document = new MosPrime
            {
                ValidDate = new DateTime(2021, 10, 03),
                MP = new MP
                {
                    MpDate = new DateTime(2021, 10, 03),
                    Ton = 1,
                    T1w = 2,
                    T2w = 3,
                    T1m = 4,
                    T2m = 5,
                    T3m = 6,
                    T4m = 7
                }
            };

            var documentXml = _mapper.Map<MosPrimeXml>(document);
            var xml = documentXml.SerializeToClearXml();
            var newDocument = xml.DeserializeFromXml<MosPrimeXml>();

            Assert.Equal(new DateTime(2021, 10, 03), newDocument.ValidDate);
            Assert.Equal(1, newDocument.MP.Ton);
            Assert.Equal(7, newDocument.MP.T4m);
        }

        [Fact]
        public void TestMosPrimeXmlFile()
        {
            string moexDocument = @"\Files\Cbr\CbrMosPrime.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<MosPrimeXml>();

            Assert.Equal(new DateTime(2021, 02, 20), newDocument.ValidDate);
        }

        [Fact]
        public void TestRoisfix()
        {
            var document = new RoisFix()
            { 
                ValidDate = new DateTime(2021, 11, 04),
                Rf = new Rf()
                {
                    D0 = new DateTime(2021, 11, 04),
                    R1w = 4.22m,
                    R2w = 4.23m,
                    R1m = 4.22m,
                    R2m = 4.22m,
                    R3m = 4.23m,
                    R6m = 4.28m
                }
            };

            var documentXml = _mapper.Map<RoisFixXml>(document);
            var xml = documentXml.SerializeToClearXml();
            var newDocument = xml.DeserializeFromXml<RoisFixXml>();

            Assert.Equal(new DateTime(2021, 11, 04), newDocument.ValidDate);
            Assert.Equal(4.22m, newDocument.Rf.R1w);
            Assert.Equal(4.28m, newDocument.Rf.R6m);
        }

        [Fact]
        public void TestRoisfoxXmlFile()
        {
            string moexDocument = @"\Files\Cbr\CbrRoisfix.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<RoisFixXml>();

            Assert.Equal(new DateTime(2021, 02, 02), newDocument.ValidDate);
        }

        [Fact]
        public void TestRuonia()
        {
            var document = new Ruonia
            {
                ValidDate = new DateTime(2021, 10, 28),
                Ro = new Ro
                {
                    D0 = new DateTime(2021, 10, 28),
                    DateUpdate = new DateTime(2021, 10, 30),
                    Ruo = 7.33m,
                    Vol = 232.65m
                }
            };

            var documentXml = _mapper.Map<RuoniaXml>(document);
            var xml = documentXml.SerializeToClearXml();
            var newDocument = xml.DeserializeFromXml<RuoniaXml>();

            Assert.Equal(new DateTime(2021, 10, 28), newDocument.ValidDate);
            Assert.Equal(7.33m, newDocument.Ro.Ruo);
            Assert.Equal(232.65m, newDocument.Ro.Vol);
        }

        [Fact]
        public void TestRuoniaXmlFile()
        {
            string moexDocument = @"\Files\Cbr\CbrRuonia.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<RuoniaXml>();

            Assert.Equal(new DateTime(2021, 10, 28), newDocument.ValidDate);
        }
    }
}
