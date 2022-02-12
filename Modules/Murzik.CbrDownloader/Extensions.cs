using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.CbrDownloader
{
    public static class Extensions
    {
        public static void AddCbrServices(this IServiceCollection services)
        {
            var cbrDownloaderMapper = AutoMapperConfiguration.Configure().CreateMapper();
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var cbrDownloader = new CbrProvider(logger, cbrDownloaderMapper);
            services.AddSingleton<ICbrDownloader>(cbrDownloader);
        }
    }
}
