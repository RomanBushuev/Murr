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
                    services.AddSingleton<ITaskActions>(new TaskActions(karmaDownloader));
                    services.AddSingleton<IServiceActions>(new ServiceActions(karmaDownloader));
                    services.AddHostedService<TimedHostedService>();
                }).UseWindowsService();
    }

    public class ServiceConfig
    {
        public string ServiceName { get; set; }

        public long Interval { get; set; }
    }
}
