using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Murzik.Interfaces;

namespace Murzik.Parser
{
    public static class Extensions 
    {
        public static IServiceCollection AddParserServices(this IServiceCollection services)
        {
            IMapper mapper = AutoMapperConfiguration.Configure().CreateMapper();
            services.AddSingleton<IJsonMoexParser>(new JsonMoexParser(mapper));
            return services;
        }
    }
}
