using AutoMapper;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Interfaces;
using Murzik.Parser.JsonEntities.Moex;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Murzik.Parser
{
    public class JsonMoexParser : IJsonMoexParser
    {
        private IMapper _mapper;

        public JsonMoexParser(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CouponInformation ConvertFromRootJson(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<CouponInformationJson[]>(json);
            return _mapper.Map<CouponInformation>(jsonObject.Last());
        }

        public CouponInformation ConvertToCouponInformation(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<CouponInformationJson>(json);
            return _mapper.Map<CouponInformation>(jsonObject);
        }

        public string ConvertToJson(CouponInformation couponInformation)
        {
            var jsonObject = _mapper.Map<CouponInformationJson>(couponInformation);
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}
