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
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                ;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json");
                    config.AddEnvironmentVariables();                    

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                    //StaticConfig = (IConfiguration)config;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).UseWindowsService();
        }

        //public static IConfiguration StaticConfig { get; private set; }
    }
}
