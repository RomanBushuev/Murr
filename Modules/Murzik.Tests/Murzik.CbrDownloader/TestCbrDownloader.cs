using AutoMapper;
using Moq;
using Murzik.CbrDownloader;
using Murzik.Interfaces;
using NLog;
using System;
using Xunit;

namespace Murzik.Tests.Murzik.CbrDownloader
{
    public class TestCbrDownloader
    {
        private ICbrDownloader _cbrDownloader;
        private IMapper _mapper;
        private Mock<ILogger> _logger;
        private DateTime constDate = new DateTime(2021, 10, 28);

        public TestCbrDownloader()
        {
            _logger = new Mock<ILogger>();
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _cbrDownloader = new CbrProvider(_logger.Object, _mapper);
        }

        [Fact]
        public async void TestDownloadForeignExchange()
        {
            await _cbrDownloader.DownloadForeignExchange(constDate);
        }

        [Fact]
        public async void TestDownloadG2()
        {
            await _cbrDownloader.DownloadG2(constDate);
        }

        [Fact]
        public async void TestDownloadKeyRate()
        {
            await _cbrDownloader.DownloadKeyRate(constDate);
        }

        [Fact]
        public async void TestDownloadMosPrime()
        {
            await _cbrDownloader.DownloadMosPrime(constDate);
        }

        [Fact]
        public async void TestDownloadRoisfix()
        {
            await _cbrDownloader.DownloadRoisfix(constDate);
        }

        [Fact]
        public async void TestDownloadRuonia()
        {
           await _cbrDownloader.DownloadRuonia(constDate);
        }
    }
}
