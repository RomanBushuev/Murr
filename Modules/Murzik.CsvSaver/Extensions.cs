using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;

namespace Murzik.CsvProvider
{
    public static class Extensions
    {
        public static void AddCsvSaverServices(this IServiceCollection services)
        {
            services.AddSingleton<ICsvSaver, CsvSaver>();
            services.AddSingleton<ICsvReaderAgent, CsvReaderAgent>();
        }
    }
}
