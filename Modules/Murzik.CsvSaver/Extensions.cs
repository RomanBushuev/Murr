using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Murzik.Entities.MoexNew;
using Murzik.Interfaces;

namespace Murzik.CsvProvider
{
    public static class Extensions
    {
        public static void AddCsvSaverServices(this IServiceCollection services, IConfiguration configuration)
        {
            var moexSettings = Options.Create(configuration.GetSection("MoexSettings").Get<MoexSettings>());
            services.AddSingleton<ICsvSaver>(new CsvSaver(moexSettings));
        }
    }
}
