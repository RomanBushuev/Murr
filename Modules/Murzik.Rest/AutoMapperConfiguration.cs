using AutoMapper;

namespace Murzik.Rest
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
