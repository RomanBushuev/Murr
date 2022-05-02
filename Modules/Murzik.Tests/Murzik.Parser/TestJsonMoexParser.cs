using AutoMapper;
using Moq;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Interfaces;
using Murzik.Parser;
using NLog;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Murzik.Tests.Murzik.Parser
{
    public class TestJsonMoexParser
    {
        private IJsonMoexParser _jsonMoexParser;
        private IMapper _mapper;
        private readonly string _root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public TestJsonMoexParser()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _jsonMoexParser = new JsonMoexParser(_mapper);
        }

        [Fact]
        public void TestConvertCoupons()
        {
            string moexDocument = @"\Files\MoexDownloadCoupons.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertToCouponInformationAndGetLast(json);
            var cursor = couponInformation.CouponCursors.First();
            Assert.Equal(0, cursor.Index);
            Assert.Equal(100, cursor.Total);
            Assert.Equal(1, cursor.PageSize);

            var coupon = couponInformation.Coupons.First();
            Assert.Equal("XS1647482436", coupon.Isin);
            Assert.Equal("PSB Finance S.A. 8.75", coupon.Name);
            Assert.Equal(500000000, coupon.IssueValue);
            Assert.Equal(new DateTime(2111, 1, 1), coupon.CouponDate);
            Assert.Null(coupon.RecordDate);
            Assert.Equal(new DateTime(2018, 8, 1), coupon.StartDate);
            Assert.Equal(1000, coupon.InitialFacevalue);
            Assert.Equal(1000, coupon.FaceValue);
            Assert.Equal("USD", coupon.FaceUnit);
            Assert.Null(coupon.Value);
            Assert.Null(coupon.ValuePrc);
            Assert.Null(coupon.ValueRub);
        }

        [Fact]
        public void TestConvertCoupons2()
        {
            string moexDocument = @"\Files\MoexDownloadCoupons2.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertToCouponInformationAndGetLast(json);
            var cursor = couponInformation.CouponCursors.First();
            Assert.Equal(72000, cursor.Index);
            Assert.Equal(108283, cursor.Total);
            Assert.Equal(100, cursor.PageSize);

            var coupons = couponInformation.Coupons;
            Assert.Equal(100, coupons.Count());            
        }

        [Fact]
        public void TestConvertToJson()
        {
            var coupon = new Coupon
            {
                Isin = "test1",
                Name = "test2",
                IssueValue = 1000,
                CouponDate = new DateTime(2022, 01, 22),
                RecordDate = new DateTime(2022, 01, 23),
                StartDate = new DateTime(2022, 01, 24),
                InitialFacevalue = 1000,
                FaceValue = 750,
                FaceUnit = "RUB",
                Value = 1,
                ValuePrc = 2,
                ValueRub = 3
            };

            var couponInformation = new CouponInformation
            {
                Coupons = new Coupon[]
                {
                    coupon
                }
            };

            var json = _jsonMoexParser.ConvertCouponToJson(couponInformation);
            var newResult = _jsonMoexParser.ConvertToCouponInformation(json).Coupons.First();
            Assert.Equal("test1", newResult.Isin);
            Assert.Equal("test2", newResult.Name);
            Assert.Equal(1000, newResult.IssueValue);
            Assert.Equal(new DateTime(2022, 01, 22), newResult.CouponDate);
            Assert.Equal(new DateTime(2022, 01, 23), newResult.RecordDate);
            Assert.Equal(new DateTime(2022, 01, 24), newResult.StartDate);
            Assert.Equal(1000, newResult.InitialFacevalue);
            Assert.Equal(750, newResult.FaceValue);
            Assert.Equal("RUB", newResult.FaceUnit);
            Assert.Equal(1, newResult.Value);
            Assert.Equal(2, newResult.ValuePrc);
            Assert.Equal(3, newResult.ValueRub);
        }

        [Fact]
        public void TestConvertAmortization()
        {
            string moexDocument = @"\Files\MoexDownloadAmortization.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertToAmortizationInformationAndGetLast(json);
            var cursor = couponInformation.AmortizationCursors.First();
            Assert.Equal(0, cursor.Index);
            Assert.Equal(100, cursor.Total);
            Assert.Equal(1, cursor.PageSize);

            var coupon = couponInformation.Amortizations.First();
            Assert.Equal("XS2075963293", coupon.Isin);
            Assert.Equal("Eurasia Capital S.A. UNDT", coupon.Name);
            Assert.Equal(200000000, coupon.IssueValue);
            Assert.Equal(new DateTime(2111, 1, 1), coupon.AmortizationDate);
            Assert.Equal(1000, coupon.FaceValue);
            Assert.Equal(1000, coupon.InitialFaceValue);
            Assert.Equal("USD", coupon.FaceUnit);
            Assert.Equal(1000, coupon.Value);
            Assert.Equal(95661.8m, coupon.ValueRub);
            Assert.Equal("maturity", coupon.DataSource);
            Assert.Equal("XS2075963293", coupon.Secid);
            Assert.Equal("RPMO", coupon.PrimaryBoardId);
        }

        [Fact]
        public void TestConvertOffer()
        {
            string moexDocument = @"\Files\MoexDownloadOffers.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertToOfferInformationAndGetLast(json);
            var cursor = couponInformation.OfferCursors.First();
            Assert.Equal(0, cursor.Index);
            Assert.Equal(100, cursor.Total);
            Assert.Equal(1, cursor.PageSize);

            var offer = couponInformation.Offers.First();
            Assert.Equal("RU000A0JUCA9", offer.Isin);
            Assert.Equal("\"ФСК ЕЭС\"(ПАО)-обл. сер.34", offer.Name);
            Assert.Equal(14000000000, offer.IssueValue);
            Assert.Equal(new DateTime(2047, 11, 11), offer.OfferDate);
            Assert.Equal(new DateTime(2047, 10, 28), offer.OfferDateStart);
            Assert.Equal(new DateTime(2047, 11, 1), offer.OfferDateEnd);
            Assert.Equal(1000, offer.FaceValue);
            Assert.Equal("RUB", offer.FaceUnit);
            Assert.Null(offer.Price);
            Assert.Null(offer.Value);
            Assert.Null(offer.Agent);
            Assert.Null(offer.Value);
            Assert.Equal("Оферта", offer.OfferType);
            Assert.Equal("RU000A0JUCA9", offer.Secid);
            Assert.Equal("TQCB", offer.PrimaryBoardId);
        }
    }
}
