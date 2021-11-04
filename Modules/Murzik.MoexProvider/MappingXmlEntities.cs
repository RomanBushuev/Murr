using AutoMapper;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using Murzik.MoexProvider.XmlEntities.Bond;
using Murzik.MoexProvider.XmlEntities.Share;

namespace Murzik.MoexProvider
{
    public class MappingXmlEntities : Profile
    {
        public MappingXmlEntities()
        {
            CreateMap<BondDocument, BondDocumentXml>()
                .ReverseMap();
            CreateMap<BondHistoryData, BondHistoryDataXml>()
                .ReverseMap();
            CreateMap<BondHistoryBondDataRow, BondHistoryBondDataRowXml>()
                .ReverseMap();
            CreateMap<BondHistoryCursorData, BondHistoryCursorDataXml>()
                .ReverseMap();
            CreateMap<BondDataRow, BondDataRowXml>()
                .ReverseMap();

            CreateMap<ShareDocument, ShareDocumentXml>()
                .ReverseMap();
            CreateMap<ShareHistoryData, ShareHistoryDataXml>()
                .ReverseMap();
            CreateMap<ShareHistoryBondDataRow, ShareHistoryBondDataRowXml>()
                .ReverseMap();
            CreateMap<ShareHistoryCursorData, ShareHistoryCursorDataXml>()
                .ReverseMap();
            CreateMap<ShareDataRow, ShareDataRowXml>()
                .ReverseMap();
        }
    }
}