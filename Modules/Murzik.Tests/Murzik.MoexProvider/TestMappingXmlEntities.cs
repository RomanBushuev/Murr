using AutoMapper;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using Murzik.MoexProvider;
using Murzik.MoexProvider.XmlEntities.Bond;
using Murzik.MoexProvider.XmlEntities.Share;
using Murzik.Utils;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Tests.Murzik.MoexProvider
{
    public class TestMappingXmlEntities
    {
        private readonly string _root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        private IMapper _mapper;
        public readonly DateTime ConstDateTime = new DateTime(2021, 10, 06);
        public TestMappingXmlEntities()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
        }

        [Fact]
        public void TestMapDocumentToDocumentXml()
        {
            var document = new BondDocument()
            {
                HistoryData = new BondHistoryData()
                {
                    Rows = new BondDataRow[]
                    {
                        new BondDataRow
                        {
                            Accint = null,
                            AdmittedQuote = null,
                            AdmittedValue = 12.0m,
                            Boardid = "ID",
                            Close = null,
                            CurrencyId = "RUB",
                            FaceValue = 14.0m,
                            High = null,
                            Legalcloseprice = null,
                            Low = null,
                            Marketprice2 = null,
                            Marketprice3 = null,
                            Marketprice3TradesValue = 20.0m,
                            MatDate = ConstDateTime,
                            MP2ValTrd = 21.0m,
                            Numtrades =22.0m,
                            Open = null,
                            Secid = "SEC_ID",
                            Shortname = "RUB_BOND",
                            Tradedate = ConstDateTime,
                            TradingSession = 10,
                            Value = 24.0m,
                            Volume = 25.0m,
                            Waprice = null,
                            YieldClose = null,
                            BeiClose = null,
                            BuyBackDate = null,
                            CbrClose = null,
                            CouponPercent = 13.0m,
                            CouponValue = 13.0m,
                            Duration = null,
                            FaceUnit = "RUB",
                            IriCPiClose = null,
                            LastTradeDate = null,
                            OfferDate = null,
                            YieldAtWap = null,
                            YieldLastCoupon = null,
                            YieldToOffer = null
                        }
                    }
                },
                HistoryCursorData = new BondHistoryCursorData()
                {
                    Rows = new BondHistoryBondDataRow[]
                    {
                        new BondHistoryBondDataRow
                        {
                            Index = 10,
                            PageSize = 100,
                            Total = 2300
                        }
                    }
                }
            };
            var documentXml = _mapper.Map<BondDocumentXml>(document);
            var str = documentXml.SerializeToXml();
            var newDocuemnt = str.DeserializeFromXml<BondDocumentXml>();
            var firstRow = newDocuemnt.HistoryData.Rows.First();

            Assert.Null(firstRow.Accint);
            Assert.Equal(ConstDateTime, firstRow.MatDate);
            Assert.Null(firstRow.LastTradeDate);
            Assert.Equal(14.0m, firstRow.FaceValue);
        }

        [Fact]
        public void TestBondHistoryDataXmlFile()
        {
            string moexDocument = @"\Files\MoexDownloadBond.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<BondDocumentXml>();

            Assert.Equal("TQOD", newDocument.HistoryData.Rows.First().Boardid);
            Assert.Equal(new DateTime(2021, 09, 30), newDocument.HistoryData.Rows.First().Tradedate);
        }

        [Fact]
        public void TestMapShareDocumentToShareDocumentXml()
        {
            var document = new ShareDocument()
            {
                HistoryData = new ShareHistoryData()
                {
                    Rows = new ShareDataRow[]
                    {
                        new ShareDataRow
                        {
                            AdmittedQuote = null,
                            AdmittedValue = 100.0m,
                            Boardid = "txt",
                            Close = null,
                            High = null,
                            Legalcloseprice = null,
                            Low = null,
                            Marketprice2 = null ,
                            Marketprice3 = null,
                            Marketprice3TradesValue = 123,
                            MP2ValTrd = 123,
                            Numtrades = 321,
                            Open = null,
                            Secid = "das",
                            Shortname = "msg",
                            Tradedate = ConstDateTime,
                            TradingSession =3,
                            Value = 321,
                            Volume = 3214,
                            Waprice =null,
                            Waval = 321
                        }
                    }
                },
                HistoryCursorData = new ShareHistoryCursorData()
                {
                    Rows = new ShareHistoryBondDataRow[]
                    {
                        new ShareHistoryBondDataRow
                        {
                            Index = 10,
                            PageSize = 100,
                            Total = 2300
                        }
                    }
                }

            };

            var documentXml = _mapper.Map<ShareDocumentXml>(document);
            var str = documentXml.SerializeToXml();
            var newDocuemnt = str.DeserializeFromXml<ShareDocumentXml>();
            var firstRow = newDocuemnt.HistoryData.Rows.First();

            Assert.Null(firstRow.AdmittedQuote);
            Assert.Equal(ConstDateTime, firstRow.Tradedate);
            Assert.Null(firstRow.Legalcloseprice);
            Assert.Equal(321.0m, firstRow.Waval);

        }

        [Fact]
        public void TestShareHistoryDataXmlFile()
        {
            string moexDocument = @"\Files\MoexDownloadShare.xml";
            string fullpath = _root + moexDocument;
            var xml = File.ReadAllText(fullpath);
            var newDocument = xml.DeserializeFromXml<ShareDocumentXml>();

            Assert.Equal("TQBR", newDocument.HistoryData.Rows.First().Boardid);
            Assert.Equal(new DateTime(2021, 02, 09), newDocument.HistoryData.Rows.First().Tradedate);
        }
    }
}
