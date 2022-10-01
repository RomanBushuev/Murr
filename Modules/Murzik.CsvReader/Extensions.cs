using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;

namespace Murzik.CsvReaderProvider
{
    public static class Extensions
    {
        public static void AddCsvReaderServices(this IServiceCollection services)
        {
            services.AddSingleton<ICsvReaderAgent, CsvReaderAgent>();
        }
    }
}
