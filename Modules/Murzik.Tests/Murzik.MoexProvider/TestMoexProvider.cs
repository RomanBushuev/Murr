using AutoMapper;
using Murzik.Interfaces;
using Murzik.MoexProvider;
using System;
using System.Linq;
using Xunit;

namespace Tests.Murzik.MoexProvider
{
    public class TestMoexProvider
    {
        private IMapper _mapper;
        private IMoexDownloader _moexDownloader;

        public TestMoexProvider()
        {
            _mapper = AutoMapperConfiguration.Configure().CreateMapper();
            _moexDownloader = new MoexDownloader(_mapper);
        }

        [Fact]
        public async void DownloadBondAsync()
        {
            var constDateWithData = new DateTime(2021, 09, 30);
            var result = await _moexDownloader.DownloadBondDataRow(constDateWithData);
            Assert.True(result.Any());

            var constDateWithoutData = new DateTime(2021, 09, 25);
            result = await _moexDownloader.DownloadBondDataRow(constDateWithoutData);
            Assert.True(!result.Any());
        }

    }
}
