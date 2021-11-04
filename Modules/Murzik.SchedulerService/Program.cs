using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Murzik.DownloaderProvider;
using Murzik.Entities;
using Murzik.Interfaces;
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
                    var service = hostContext.Configuration.GetSection("SchedulerServiceConfige").Get<SchedulerServiceConfige>();
                    var schedulerMapper = AutoMapperConfiguration.Configure().CreateMapper();
                    services.AddSingleton<ISchedulerActions>(new SchedulerActions(service.KarmaDownloader, schedulerMapper, logger));
                    services.AddSingleton<IServiceActions>(new ServiceActions(service.KarmaDownloader, schedulerMapper));
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }
}
