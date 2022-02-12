using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;

namespace Murzik.XmlSaver
{
    public static class Extensions
    {
        public static void AddXmlSaverServices(this IServiceCollection services)
        {
            services.AddSingleton<IXmlSaver>(new XmlSaver());
        }
    }
}
