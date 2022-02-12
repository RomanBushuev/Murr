using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.AlgorithmServiceActions
{
    public static class Extensions
    {
        public static void AddAlgorithmServiceActionsServices(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var taskActions = sp.GetRequiredService<ITaskActions>();
            var serviceActions = sp.GetRequiredService<IServiceActions>();
            var logger = sp.GetRequiredService<ILogger>();
            var calculationFactory = sp.GetRequiredService<ICalculationFactory>();

            services.AddSingleton<IAlgorithmServiceProvider>(new AlgorithmServiceProvider(taskActions, serviceActions, logger, calculationFactory));
        }
    }
}
