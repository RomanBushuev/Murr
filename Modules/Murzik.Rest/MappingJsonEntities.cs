using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.Rest.Views;

namespace Murzik.Rest
{
    public class MappingJsonEntities : Profile
    {
        public MappingJsonEntities()
        {
            CreateMap<FinDataSources, FinDataSourcesView>()
                .ReverseMap();
        }
    }
}
