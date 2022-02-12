using AutoMapper;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Parser.JsonEntities.Moex;

namespace Murzik.Parser
{
    internal class MappingJsonEntities : Profile
    {
        public MappingJsonEntities()
        {
            CreateMap<CouponJson, Coupon>()
                .ReverseMap();
            CreateMap<CouponCursorJson, CouponCursor>()
                .ReverseMap();
            CreateMap<CouponInformationJson, CouponInformation>()
                .ReverseMap();
        }
    }
}
