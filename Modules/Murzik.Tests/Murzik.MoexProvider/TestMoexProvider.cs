using AutoMapper;
using Moq;
using Murzik.Interfaces;
using Murzik.MoexProvider;
using Murzik.Entities.MoexNew.Coupon;
using NLog;
using System;
using Xunit;
using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Offer;

namespace Tests.Murzik.MoexProvider
{
    public class TestMoexProvider
    {
        private IMapper _mapper;
        private IMoexDownloader _moexDownloader;
        private Mock<IJsonMoexParser> _jsonMoexParser;
        private Mock<ILogger> _logger;

        public TestMoexProvider()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _logger = new Mock<ILogger>();
            _jsonMoexParser = new Mock<IJsonMoexParser>();
            _jsonMoexParser.Setup(z => z.ConvertToCouponInformationAndGetLast(It.IsAny<string>())).Returns(new CouponInformation
            {
                CouponCursors = new[]
                {
                    new CouponCursor
                    {
                        Index = 0,
                        PageSize = 100,
                        Total = 100
                    }
                },
                Coupons = new[]
                {
                    new Coupon
                    {
                        
                    }
                }
            });

            _jsonMoexParser.Setup(z => z.ConvertToAmortizationInformationAndGetLast(It.IsAny<string>())).Returns(new AmortizationInformation
            {
                AmortizationCursors = new[]
                {
                    new AmortizationCursor
                    {
                        Index = 0,
                        PageSize = 100,
                        Total = 100
                    }
                },
                Amortizations = new[]
                {
                    new Amortization
                    {

                    }
                }
            });

            _jsonMoexParser.Setup(z=>z.ConvertToOfferInformationAndGetLast(It.IsAny<string>())).Returns(new OfferInformation
            {
               OfferCursors = new[]
               {
                   new OfferCursor
                   {
                       Index = 0,
                       PageSize = 100,
                       Total = 100
                   }
               },
               Offers = new []
               {
                   new Offer
                   {

                   }
               }
            });
            _moexDownloader = new MoexDownloader(_mapper, _logger.Object, _jsonMoexParser.Object);
        }

        [Fact]
        public async void DownloadBondAsync()
        {
            var constDateWithData = new DateTime(2021, 09, 30);
            var result = await _moexDownloader.DownloadBondDataRowAsync(constDateWithData);
            Assert.NotEmpty(result);

            var constDateWithoutData = new DateTime(2021, 09, 25);
            result = await _moexDownloader.DownloadBondDataRowAsync(constDateWithoutData);
            Assert.Empty(result);
        }

        [Fact]
        public async void DownloadShareAsync()
        {
            var constDateWithData = new DateTime(2021, 09, 30);
            var result = await _moexDownloader.DownloadShareDataRowAsync(constDateWithData);
            Assert.NotEmpty(result);

            var constDateWithoutData = new DateTime(2021, 09, 25);
            result = await _moexDownloader.DownloadShareDataRowAsync(constDateWithoutData);
            Assert.Empty(result);
        }

        [Fact]
        public async void DownloadCouponsAsync()
        {
            var result = await _moexDownloader.DownloadCouponsAsync(1);
            Assert.Single(result);
        }

        [Fact]
        public async void DownloadAmortizationsAsync()
        {
            var result = await _moexDownloader.DownloadAmortizationsAsync(1);
            Assert.Single(result);
        }

        [Fact]
        public async void TestDownloadOffersAsync()
        {
            var result = await _moexDownloader.DownloadOffersAsync(1);
            Assert.Single(result);
        }
    }
}
