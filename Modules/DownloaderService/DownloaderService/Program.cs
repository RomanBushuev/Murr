using AutoMapper;
using DownloaderProvider;
using KarmaCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DownloaderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Service"));
                    string karmaDownloader = hostContext.Configuration.GetSection("DataProviders").GetValue<string>("KarmaDownloader");
                    string karmaSaver = hostContext.Configuration.GetSection("DataProviders").GetValue<string>("KarmaSaver");
                    services.AddSingleton<ITaskActions>(new TaskActions(karmaDownloader));
                    services.AddSingleton<IServiceActions>(new ServiceActions(karmaDownloader));
                    ICbrXmlRepository cbrXmlRepository = new CbrRepository();
                    services.AddSingleton<ICbrXmlRepository>(cbrXmlRepository);

                    var config = AutoMapperConfiguration.Configure();
                    IMapper mapper = config.CreateMapper();
                    services.AddSingleton<IMapper>(mapper);

                    IFinInstrumentRepository finInstrumentRepository = new FinInstrumentRepository(mapper);
                    IFinDataSourceRepository finDataSourceRepository = new FinDataSourceRepository(mapper);

                    IMarkerRepository markerRepository = new MarkerRepository(karmaSaver, finInstrumentRepository, finDataSourceRepository);
                    services.AddSingleton<IMarkerRepository>(markerRepository);
                    services.AddSingleton<ICalculationFactory>(new CalculationFactory(cbrXmlRepository, markerRepository));
                    services.AddHostedService<TimedHostedService>();
                }).UseWindowsService();
    }

    public class ServiceConfig
    {
        public string ServiceName { get; set; }

        public long Interval { get; set; }
    }
}
