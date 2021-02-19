using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace KarmaScheduler
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
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();                    

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                    //StaticConfig = (IConfiguration)config;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<ServiceConfig>(hostContext.Configuration.GetSection("Service"));
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }
    }

    public class ServiceConfig
    {
        public string ServiceName { get; set; }

        public long Interval { get; set; }

        public string Configuration { get; set; }

        public string KarmaAdmin { get; set; }

        public string KarmaDownloader { get; set; }
    }
}
