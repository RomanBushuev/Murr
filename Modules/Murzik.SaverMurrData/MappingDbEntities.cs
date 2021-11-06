using AutoMapper;
using Murzik.Entities.MurrData;
using Murzik.SaverMurrData.DbEntities;
using System.Collections.Generic;
using System.Linq;

namespace Murzik.SaverMurrData
{
    public class MappingDbEntities : Profile
    {
        public MappingDbEntities()
        {
            CreateMap<FinInstrument, DbFinInstrument>()
                .ForMember(z => z.FinInstrumentIdent, x => x.MapFrom(source => source.FinIdent))
                .ForMember(z => z.DataSourceId, x => x.MapFrom(source => source.DataSourceId));

            CreateMap<FinInstrument, IReadOnlyCollection<DbFinDataValue>>()
                .ConstructUsing(z => z.FinDataValues.Select(x => new DbFinDataValue()
                {
                    FinAttributeIdent = x.FinAttriubte,
                    FinInstrumentId = z.FinId.Value,
                    FromDate = x.FromDate,
                    Value = x.Value
                }).ToArray());

            CreateMap<FinInstrument, IReadOnlyCollection<DbFinNumericValue>>()
                .ConstructUsing(z => z.FinNumericValues.Select(x => new DbFinNumericValue()
                {
                    FinAttributeIdent = x.FinAttriubte,
                    FinInstrumentId = z.FinId.Value,
                    FromDate = x.FromDate,
                    Value = x.Value
                }).ToArray());

            CreateMap<FinInstrument, IReadOnlyCollection<DbFinStringValue>>()
                .ConstructUsing(z => z.FinStringValues.Select(x => new DbFinStringValue()
                {
                    FinAttributeIdent = x.FinAttriubte,
                    FinInstrumentId = z.FinId.Value,
                    FromDate = x.FromDate,
                    Value = x.Value
                }).ToArray());

            CreateMap<FinInstrument, IReadOnlyCollection<DbFinTimeSeries>>()
               .ConstructUsing(z => z.FinTimeSerieses.Select(x => new DbFinTimeSeries()
               {
                   FinAttributeIdent = x.FinAttriubte,
                   FinInstrumentId = z.FinId.Value,
                   Date = x.Date,
                   Value = x.Value
               }).ToArray());
        }
    }
}
