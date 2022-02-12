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
        public void TestConvertFromTemplateJson()
        {
            string moexDocument = @"\Files\MoexDownloadCoupons.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertFromRootJson(json);
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
        public void TestConvertFromTemplateJson2()
        {
            string moexDocument = @"\Files\MoexDownloadCoupons2.json";
            string fullpath = _root + moexDocument;
            var json = File.ReadAllText(fullpath).Trim();
            var couponInformation = _jsonMoexParser.ConvertFromRootJson(json);
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

            var json = _jsonMoexParser.ConvertToJson(couponInformation);
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
    }
}
