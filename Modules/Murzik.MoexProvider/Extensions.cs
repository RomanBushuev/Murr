using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Murzik.Entities.MoexNew;
using Murzik.Interfaces;
using NLog;

namespace Murzik.MoexProvider
{
    public static class Extensions
    {
        public static void AddMoexServices(this IServiceCollection services, IConfiguration configuration)
        {
            var moexMapper = AutoMapperConfiguration.Configure().CreateMapper();
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var jsonParser = sp.GetRequiredService<IJsonMoexParser>();
            var moexSettings = Options.Create(configuration.GetSection("MoexSettings").Get<MoexSettings>());
            var moexDownloader = new MoexDownloader(moexMapper, logger, jsonParser, moexSettings);
            services.AddSingleton<IMoexDownloader>(moexDownloader);
        }
    }
}
