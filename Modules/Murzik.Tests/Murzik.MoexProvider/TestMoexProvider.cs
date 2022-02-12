using AutoMapper;
using Moq;
using Murzik.Interfaces;
using Murzik.MoexProvider;
using Murzik.Entities.MoexNew.Coupon;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
            _jsonMoexParser.Setup(z => z.ConvertFromRootJson(It.IsAny<string>())).Returns(new CouponInformation
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
            _moexDownloader = new MoexDownloader(_mapper, _logger.Object, _jsonMoexParser.Object);
        }

        [Fact]
        public async void DownloadBondAsync()
        {
            var constDateWithData = new DateTime(2021, 09, 30);
            var result = await _moexDownloader.DownloadBondDataRow(constDateWithData);
            Assert.NotEmpty(result);

            var constDateWithoutData = new DateTime(2021, 09, 25);
            result = await _moexDownloader.DownloadBondDataRow(constDateWithoutData);
            Assert.Empty(result);
        }

        [Fact]
        public async void DownloadShareAsync()
        {
            var constDateWithData = new DateTime(2021, 09, 30);
            var result = await _moexDownloader.DownloadShareDataRow(constDateWithData);
            Assert.NotEmpty(result);

            var constDateWithoutData = new DateTime(2021, 09, 25);
            result = await _moexDownloader.DownloadShareDataRow(constDateWithoutData);
            Assert.Empty(result);
        }

        [Fact]
        public async void DownloadCouponsAsync()
        {
            var result = await _moexDownloader.DownloadCoupons(1);
            Assert.Single(result);
        }

    }

    public class ItemStringVer
    {
        public string DictItemField { get; set; }

        public DateTime FirstDate { get; set; }

        public DateTime SecondDate { get; set; }

        public string Value { get; set; }
    }

    public class TestItemStringVer
    {
        [Fact]
        public void TestSort()
        {
            var firstList = new List<ItemStringVer>()
            {
                new ItemStringVer
                {
                    DictItemField = "any",
                    FirstDate = new DateTime(2021, 01, 01),
                    SecondDate = new DateTime(2021, 01,02),
                    Value = "a"
                },
                new ItemStringVer
                {
                    DictItemField = "any",
                    FirstDate = new DateTime(2021, 01, 04),
                    SecondDate = new DateTime(2021, 01, 06),
                    Value = "b"
                }
            }.OrderBy(z=>z.FirstDate).ToList();
            var secondList = new List<ItemStringVer>()
            { 
                new ItemStringVer
                {
                    DictItemField = "any",
                    FirstDate = new DateTime(2021, 01, 02),
                    SecondDate = new DateTime(2021, 01, 04),
                    Value = "c"
                },
                new ItemStringVer
                {
                    DictItemField = "any",
                    FirstDate = new DateTime(2021, 01, 05),
                    SecondDate = new DateTime(2021, 01, 07),
                    Value = "d"
                },
                new ItemStringVer
                {
                    DictItemField = "any",
                    FirstDate = new DateTime(2021, 01, 09),
                    SecondDate = new DateTime(2021, 01, 11),
                    Value = "e"
                }
            }.OrderBy(z => z.FirstDate).ToList();

            var allStarts = firstList.Select(z => z.FirstDate)
                .Concat(secondList.Select(z => z.FirstDate))
                .OrderBy(z => z);

            var allEnds = firstList.Select(z => z.SecondDate)
                .Concat(secondList.Select(z => z.SecondDate))
                .OrderBy(z => z);

            var allDates = allStarts.Concat(allEnds).OrderBy(z => z).ToArray();

            var startDate = allStarts.First();
            var indexDate = 0;
            var indexFirst = 0;
            DateTime? firstdate1 = null;
            DateTime? firstdate2 = null;
            var indexSecond = 0;
            var endDate = allStarts.Last();

            var list = new List<ItemStringVer>();
            while (true)
            {
                if (indexFirst < firstList.Count)
                    firstdate1 = firstList.ElementAt(indexFirst).FirstDate;
                else
                    firstdate1 = null;

                if (indexSecond < secondList.Count)
                    firstdate2 = secondList.ElementAt(indexSecond).FirstDate;
                else
                    firstdate2 = null;

                //первая дата есть
                if (firstdate1.HasValue && !firstdate2.HasValue)
                {
                    list.Add(new ItemStringVer
                    {
                        DictItemField = "any",
                        FirstDate = firstdate1.Value,
                        SecondDate = firstList.First(z => z.SecondDate > firstdate1.Value).SecondDate > allEnds.First(z => z > firstdate1.Value)
                            ? secondList.First(z => z.SecondDate > firstdate1.Value).SecondDate
                            : allEnds.First(z => z > firstdate1.Value),
                        Value = firstList.ElementAt(indexFirst).Value
                    });
                    indexFirst++;
                }
                //вторая дата есть
                else if (!firstdate1.HasValue && firstdate2.HasValue)
                {
                    list.Add(new ItemStringVer
                    {
                        DictItemField = "any",
                        FirstDate = firstdate2.Value,
                        SecondDate = secondList.First(z => z.SecondDate > firstdate2.Value).SecondDate > allEnds.First(z=>z > firstdate2.Value) 
                            ? secondList.First(z => z.SecondDate > firstdate2.Value).SecondDate
                            : allEnds.First(z => z > firstdate2.Value),
                        Value = secondList.ElementAt(indexSecond).Value
                    });
                    indexSecond++;
                }
                //даты сходяться 
                else if (firstdate1.HasValue && firstdate2.HasValue && startDate == firstdate1.Value && startDate == firstdate2.Value)
                {
                    var fv = firstList.ElementAt(indexFirst);
                    var sv = secondList.ElementAt(indexSecond);
                    if (fv.Value != sv.Value)
                        throw new Exception();

                    indexFirst++;
                    indexSecond++;
                    //два конца - выбрать лучший 

                }
                //первая дата меньше второй
                else if (firstdate1.HasValue && firstdate2.HasValue && firstdate1.Value < firstdate2.Value)
                {
                    list.Add(new ItemStringVer
                    {
                        DictItemField = "any",
                        FirstDate = firstdate1.Value,
                        SecondDate = allDates.First(z => z > firstdate1.Value),
                        Value = firstList.ElementAt(indexFirst).Value
                    });
                    indexFirst++;
                }
                //вторая дата меньше первой
                else if (firstdate1.HasValue && firstdate2.HasValue && firstdate1.Value > firstdate2.Value)
                {
                    list.Add(new ItemStringVer
                    {
                        DictItemField = "any",
                        FirstDate = firstdate2.Value,
                        SecondDate = allDates.First(z => z > firstdate2.Value),
                        Value = secondList.ElementAt(indexSecond).Value
                    });
                    indexSecond++;
                }

                indexDate++;
                //конец
                if (startDate == endDate)
                    break;
                startDate = allStarts.ElementAt(indexDate);                
            }
            Console.WriteLine(list.Count);
        }
    }
}
