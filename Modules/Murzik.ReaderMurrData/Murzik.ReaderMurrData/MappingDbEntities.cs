using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.ReaderMurrData.DbEntities;

namespace Murzik.ReaderMurrData
{
    internal class MappingDbEntities : Profile
    {
        public MappingDbEntities()
        {
            CreateMap<FinDataSources, DbFinDataSources>()
                .ReverseMap();
        }
    }
}
