using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Murzik.DownloaderProvider;
using Murzik.Entities;
using NLog;
using NLog.Extensions.Logging;

namespace Murzik.SchedulerService
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
                    services.Configure<SchedulerServiceConfige>(hostContext.Configuration.GetSection("SchedulerServiceConfige"));
                    services.AddDownloaderServices(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }
}
