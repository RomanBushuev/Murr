using AutoMapper;
using Murzik.Entities.MoexNew;
using Murzik.MoexProvider;
using Murzik.MoexProvider.XmlEntities;
using Murzik.Utils;
using System;
using System.Linq;
using Xunit;

namespace Tests.Murzik.MoexProvider
{
    public class TestMappingXmlEntities
    {
        private IMapper _mapper;
        public readonly DateTime ConstDateTime = new DateTime(2021, 10, 06);

        
        public TestMappingXmlEntities()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
        }

        [Fact]
        public void TestMapDocumentToDocumentXml()
        {
            var document = new Document()
            {
                HistoryData = new HistoryData()
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
                HistoryCursorData = new HistoryCursorData()
                {
                    Rows = new HistoryBondDataRow[]
                    {
                        new HistoryBondDataRow
                        {
                            Index = 10,
                            PageSize = 100,
                            Total = 2300
                        }
                    }
                }
            };
            var documentXml = _mapper.Map<DocumentXml>(document);
            var str = documentXml.SerializeToXml();
            var newDocuemnt = str.DeserializeFromXml<DocumentXml>();
            var firstRow = newDocuemnt.HistoryData.Rows.First();

            Assert.Null(firstRow.Accint);
            Assert.Equal(ConstDateTime, firstRow.MatDate);
            Assert.Null(firstRow.LastTradeDate);
            Assert.Equal(14.0m, firstRow.FaceValue);
        }
    }
}
