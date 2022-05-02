using AutoMapper;
using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Offer;
using Murzik.Parser.JsonEntities.Moex;
using Murzik.Parser.JsonEntities.Moex.Offer;

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

            CreateMap<AmortizationJson, Amortization>()
                .ReverseMap();
            CreateMap<AmortizationCursorJson, AmortizationCursor>()
                .ReverseMap();
            CreateMap<AmortizationInformationJson, AmortizationInformation>()
                .ReverseMap();

            CreateMap<OfferJson, Offer>()
                .ReverseMap();
            CreateMap<OfferCursorJson, OfferCursor>()
                .ReverseMap();
            CreateMap<OfferInformationJson, OfferInformation>()
                .ReverseMap();
        }
    }
}
