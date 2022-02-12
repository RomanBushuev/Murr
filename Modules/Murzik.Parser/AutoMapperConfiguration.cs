using AutoMapper;

namespace Murzik.Parser
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingJsonEntities>();
            });

            return config;
        }
    }
}
