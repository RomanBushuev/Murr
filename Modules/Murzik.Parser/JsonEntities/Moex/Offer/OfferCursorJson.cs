using Newtonsoft.Json;

namespace Murzik.Parser.JsonEntities.Moex.Offer
{
    public class OfferCursorJson
    {
        [JsonProperty("INDEX")]
        public long Index { get; set; }

        [JsonProperty("TOTAL")]
        public long Total { get; set; }

        [JsonProperty("PAGESIZE")]
        public long PageSize { get; set; }
    }
}
