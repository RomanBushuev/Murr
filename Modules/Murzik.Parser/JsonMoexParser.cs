using AutoMapper;
using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Offer;
using Murzik.Interfaces;
using Murzik.Parser.JsonEntities.Moex;
using Murzik.Parser.JsonEntities.Moex.Offer;
using Newtonsoft.Json;
using System.Linq;

namespace Murzik.Parser
{
    /// <inheritdoc cref="IJsonMoexParser"/>
    public class JsonMoexParser : IJsonMoexParser
    {
        private IMapper _mapper;

        public JsonMoexParser(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public CouponInformation ConvertToCouponInformationAndGetLast(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<CouponInformationJson[]>(json);
            return _mapper.Map<CouponInformation>(jsonObject.Last());
        }

        /// <inheritdoc/>
        public CouponInformation ConvertToCouponInformation(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<CouponInformationJson>(json);
            return _mapper.Map<CouponInformation>(jsonObject);
        }

        /// <inheritdoc/>
        public AmortizationInformation ConvertToAmortizationInformationAndGetLast(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<AmortizationInformationJson[]>(json);
            return _mapper.Map<AmortizationInformation>(jsonObject.Last());
        }

        public AmortizationInformation ConvertToAmortizationInformation(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<AmortizationInformationJson>(json);
            return _mapper.Map<AmortizationInformation>(jsonObject);
        }

        /// <inheritdoc/>
        public string ConvertCouponToJson(CouponInformation couponInformation)
        {
            var jsonObject = _mapper.Map<CouponInformationJson>(couponInformation);
            return JsonConvert.SerializeObject(jsonObject);
        }

        /// <inheritdoc/>
        public OfferInformation ConvertToOfferInformationAndGetLast(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<OfferInformationJson[]>(json);
            return _mapper.Map<OfferInformation>(jsonObject.Last());
        }
    }
}
