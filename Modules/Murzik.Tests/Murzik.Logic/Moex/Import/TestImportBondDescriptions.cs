using Moq;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MurrData;
using Murzik.Interfaces;
using Murzik.Logic.Moex.Converter;
using Murzik.Logic.Moex.Import;
using Murzik.Utils;
using System;
using Xunit;

namespace Murzik.Tests.Murzik.Logic.Moex.Import
{
    public class TestImportBondDescriptions : TestBaseAlgorithm
    {
        IAlgorithm _algorithm;
        Mock<IConverterFactory> _converterFactory;
        Mock<ISaverMurrData> _saverMurrData;
        Mock<ICsvReaderAgent> _csvReaderAgent;

        public TestImportBondDescriptions()
            : base()
        {
            _csvReaderAgent = new Mock<ICsvReaderAgent>();
            _csvReaderAgent.Setup(z => z.ReadBondDescriptionAsync(It.IsAny<string>()))
                .ReturnsAsync(new[]
                {
                    new BondDescription()
                    {

                    }
                });
            _converterFactory = new Mock<IConverterFactory>();
            _converterFactory.Setup(x => x.GetConverter(It.IsAny<Type>()))
                .Returns(new ConverterMoexBonds());

            _saverMurrData = new Mock<ISaverMurrData>();
            _saverMurrData.Setup(z => z.Save(It.IsAny<PackValues>()));
        }

        [Fact]
        public void TestLogic()
        {
            _algorithm = new ImportBondDescriptions(_logger.Object,
                _taskActions.Object,
                _converterFactory.Object,
                _saverMurrData.Object,
                _csvReaderAgent.Object);

            var varialbes = _algorithm.GetParamDescriptors();
            varialbes.SetStr(ImportBondDescriptions.File, "test.csv");
            varialbes.SetDat(ImportBondDescriptions.RunDateTime, DateTime.Today);

            _algorithm.Run();
        }
    }
}
