using AutoMapper;
using Murzik.Entities.MoexNew;
using Murzik.MoexProvider.XmlEntities;

namespace Murzik.MoexProvider
{
    public class MappingXmlEntities : Profile
    {
        public MappingXmlEntities()
        {
            CreateMap<Document, DocumentXml>()
                .ReverseMap();
            CreateMap<HistoryData, HistoryDataXml>()
                .ReverseMap();
            CreateMap<HistoryBondDataRow, HistoryBondDataRowXml>()
                .ReverseMap();
            CreateMap<HistoryCursorData, HistoryCursorDataXml>()
                .ReverseMap();
            CreateMap<BondDataRow, BondDataRowXml>()
                .ReverseMap();
        }
    }
}