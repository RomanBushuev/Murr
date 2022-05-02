using Newtonsoft.Json;
using System.Collections.Generic;

namespace Murzik.Parser.JsonEntities.Moex
{
    public class CouponInformationJson
    {
        [JsonProperty("charsetinfo")]
        public Charsetinfo Charsetinfo { get; set; }

        [JsonProperty("coupons")]
        public List<CouponJson> Coupons { get; set; }

        [JsonProperty("coupons.cursor")]
        public List<CouponCursorJson> CouponCursors { get; set; }
    }
}
