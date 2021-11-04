using AutoMapper;

namespace Murzik.MoexProvider
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingXmlEntities>();
            });

            return config;
        }
    }
}
