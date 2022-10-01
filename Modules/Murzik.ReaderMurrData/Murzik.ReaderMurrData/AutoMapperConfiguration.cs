using AutoMapper;

namespace Murzik.ReaderMurrData
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingDbEntities>();
            });

            return config;
        }
    }
}
