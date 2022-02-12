using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Murzik.Entities;
using Murzik.Interfaces;
using NLog;

namespace Murzik.SaverMurrData
{
    public static class Extensions
    {
        public static void AddSaverMurrDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dataProvider = configuration.GetSection("DataProvider").Get<DataProvider>();
            var saverMurrMapper = AutoMapperConfiguration.Configure().CreateMapper();
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var saverMurrData = new SaverMurrProvider(logger, saverMurrMapper, dataProvider.KarmaSaver);
            services.AddSingleton<ISaverMurrData>(saverMurrData);
        }
    }
}
