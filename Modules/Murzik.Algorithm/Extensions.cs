using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.AlgorithmServiceActions
{
    public static class Extensions
    {
        public static void AddAlgorithmServiceActionsServices(this IServiceCollection services)
        {
            services.AddSingleton<IAlgorithmServiceProvider, AlgorithmServiceProvider>();
        }
    }
}
