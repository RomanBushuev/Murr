using Newtonsoft.Json;

namespace Murzik.Parser.JsonEntities.Moex
{
    public class CouponCursorJson
    {
        [JsonProperty("INDEX")]
        public long Index { get; set; }

        [JsonProperty("TOTAL")]
        public long Total { get; set; }

        [JsonProperty("PAGESIZE")]
        public long PageSize { get; set; }
    }
}
