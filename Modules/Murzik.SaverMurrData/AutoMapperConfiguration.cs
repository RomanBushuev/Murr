using AutoMapper;

namespace Murzik.SaverMurrData
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
