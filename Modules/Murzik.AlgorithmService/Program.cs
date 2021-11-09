using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Murzik.Algorithm;
using Murzik.CbrDownloader;
using Murzik.DownloaderProvider;
using Murzik.Entities;
using Murzik.Interfaces;
using Murzik.Logic;
using Murzik.MoexProvider;
using Murzik.SaverMurrData;
using NLog;
using NLog.Extensions.Logging;

namespace Murzik.AlgorithmService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    var nLogConfig = hostContext.Configuration.GetSection("NLog");
                    LogManager.Configuration = new NLogLoggingConfiguration(nLogConfig);
                    ILogger logger = LogManager.GetCurrentClassLogger();
                    services.AddSingleton(logger);
                    services.Configure<AlgorithmServiceConfige>(hostContext.Configuration.GetSection("AlgorithmServiceConfige"));
                    var dataProvider = hostContext.Configuration.GetSection("DataProvider").Get<DataProvider>();
                    IMapper mapper = DownloaderProvider.AutoMapperConfiguration.Configure().CreateMapper();
                    IServiceActions serviceActions = new ServiceActions(dataProvider.KarmaDownloader, mapper);
                    ITaskActions taskActions = new TaskActions(dataProvider.KarmaDownloader, mapper);
                    IMapper cbrDownloaderMapper = CbrDownloader.AutoMapperConfiguration.Configure().CreateMapper();
                    ICbrDownloader cbrDownloader = new CbrProvider(logger, cbrDownloaderMapper);
                    IMapper moexMapper = MoexProvider.AutoMapperConfiguration.Configure().CreateMapper();
                    IMoexDownloader moexDownloader = new MoexDownloader(moexMapper, logger);
                    IXmlSaver xmlSaver = new XmlSaver.XmlSaver();
                    ICsvSaver csvSaver = new CsvSaver.CsvSaver();
                    IConverterFactory converterFactory = new ConverterFactory();
                    IMapper saverMurrMapper = SaverMurrData.AutoMapperConfiguration.Configure().CreateMapper();
                    ISaverMurrData saverMurrData = new SaverMurrProvider(logger, saverMurrMapper, dataProvider.KarmaSaver);
                    ICalculationFactory calculationFactory = new CalculationFactory(logger, taskActions, cbrDownloader, moexDownloader, xmlSaver, csvSaver, converterFactory, saverMurrData);
                    services.AddSingleton<IAlgorithmServiceProvider>(new AlgorithmServiceProvider(taskActions, serviceActions, logger, calculationFactory));
                    var schedulerMapper = DownloaderProvider.AutoMapperConfiguration.Configure().CreateMapper();
                    services.AddSingleton<IServiceActions>(new ServiceActions(dataProvider.KarmaDownloader, schedulerMapper));
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }
}
