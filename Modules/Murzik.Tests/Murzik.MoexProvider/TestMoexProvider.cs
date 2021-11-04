using AutoMapper;
using Moq;
using Murzik.Interfaces;
using Murzik.MoexProvider;
using NLog;
using System;
using Xunit;

namespace Tests.Murzik.MoexProvider
{
    public class TestMoexProvider
    {
        private IMapper _mapper;
        private IMoexDownloader _moexDownloader;
        private Mock<ILogger> _logger;

        public TestMoexProvider()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _logger = new Mock<ILogger>();
            _moexDownloader = new MoexDownloader(_mapper, _logger.Object);
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

    }
}
