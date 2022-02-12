using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Murzik.Entities;
using Murzik.Interfaces;
using NLog;

namespace Murzik.DownloaderProvider
{
    public static class Extensions
    {
        public static void AddDownloaderServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dataProvider = configuration.GetSection("DataProvider").Get<DataProvider>();
            var mapper = AutoMapperConfiguration.Configure().CreateMapper();
            var serviceActions = new ServiceActions(dataProvider.KarmaDownloader, mapper);
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var scheduler = new SchedulerActions(dataProvider.KarmaDownloader, mapper, logger);
            var taskActions = new TaskActions(dataProvider.KarmaDownloader, mapper);
            services.AddSingleton<IServiceActions>(serviceActions);
            services.AddSingleton<ISchedulerActions>(scheduler);
            services.AddSingleton<ITaskActions>(taskActions);
        }
    }
}
