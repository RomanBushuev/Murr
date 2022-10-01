using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Murzik.AlgorithmServiceActions;
using Murzik.CbrDownloader;
using Murzik.CsvProvider;
using Murzik.DownloaderProvider;
using Murzik.Entities;
using Murzik.Entities.MoexNew;
using Murzik.Logic;
using Murzik.MoexProvider;
using Murzik.Parser;
using Murzik.SaverMurrData;
using Murzik.XmlSaver;
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
                    services.AddSingleton<ILogger>(logger);
                    services.Configure<AlgorithmServiceConfige>(hostContext.Configuration.GetSection("AlgorithmServiceConfige"));
                    services.Configure<MoexSettings>(hostContext.Configuration.GetSection("MoexSettings"));

                    services.AddDownloaderServices(hostContext.Configuration);
                    services.AddParserServices();
                    services.AddCbrServices();
                    services.AddMoexServices(hostContext.Configuration);
                    services.AddXmlSaverServices();
                    services.AddCsvSaverServices();
                    services.AddSaverMurrDataServices(hostContext.Configuration);
                    services.AddLogicServices();
                    services.AddAlgorithmServiceActionsServices();

                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }
}
