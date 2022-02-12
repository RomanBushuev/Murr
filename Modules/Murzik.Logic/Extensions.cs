using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;
using NLog;

namespace Murzik.Logic
{
    public static class Extensions
    {
        public static void AddLogicServices(this IServiceCollection services)
        {
            var converterFactory = new ConverterFactory();
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var taskActions = sp.GetRequiredService<ITaskActions>();
            var cbrDownloader = sp.GetRequiredService<ICbrDownloader>();
            var moexDownloader = sp.GetRequiredService<IMoexDownloader>();
            var xmlSaver = sp.GetRequiredService<IXmlSaver>();
            var csvSaver = sp.GetRequiredService<ICsvSaver>();
            var saverMurrData = sp.GetRequiredService<ISaverMurrData>();

            var calculationFactory = new CalculationFactory(logger, taskActions, cbrDownloader, moexDownloader, xmlSaver, csvSaver, converterFactory, saverMurrData);
            services.AddSingleton<ICalculationFactory>(calculationFactory);
        }
    }
}
