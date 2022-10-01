using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.Logic
{
    public static class Extensions
    {
        public static void AddLogicServices(this IServiceCollection services)
        {
            services.AddSingleton<IConverterFactory, ConverterFactory>();
            services.AddSingleton<ICalculationFactory, CalculationFactory>();
        }
    }
}
