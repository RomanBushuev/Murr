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
                    ICbrDownloader cbrDownloader = new CbrProvider(logger);
                    IMapper moexMapper = MoexProvider.AutoMapperConfiguration.Configure().CreateMapper();
                    IMoexDownloader moexDownloader = new MoexDownloader(moexMapper, logger);
                    IXmlSaver xmlSaver = new XmlSaver.XmlSaver();
                    ICsvSaver csvSaver = new CsvSaver.CsvSaver();
                    ICalculationFactory calculationFactory = new CalculationFactory(logger, taskActions, cbrDownloader, moexDownloader, xmlSaver, csvSaver);
                    services.AddSingleton<IAlgorithmServiceProvider>(new AlgorithmServiceProvider(taskActions, serviceActions, logger, calculationFactory));
                    var schedulerMapper = DownloaderProvider.AutoMapperConfiguration.Configure().CreateMapper();
                    services.AddSingleton<IServiceActions>(new ServiceActions(dataProvider.KarmaDownloader, schedulerMapper));
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }
}
