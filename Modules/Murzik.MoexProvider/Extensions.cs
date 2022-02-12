using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.MoexProvider
{
    public static class Extensions
    {
        public static void AddMoexServices(this IServiceCollection services)
        {
            var moexMapper = AutoMapperConfiguration.Configure().CreateMapper();
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var jsonParser = sp.GetRequiredService<IJsonMoexParser>();
            var moexDownloader = new MoexDownloader(moexMapper, logger, jsonParser);
            services.AddSingleton<IMoexDownloader>(moexDownloader);
        }
    }
}
