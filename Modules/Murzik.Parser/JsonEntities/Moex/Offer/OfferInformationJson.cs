using Newtonsoft.Json;
using System.Collections.Generic;

namespace Murzik.Parser.JsonEntities.Moex.Offer
{
    public class OfferInformationJson
    {
        [JsonProperty("charsetinfo")]
        public Charsetinfo Charsetinfo { get; set; }

        [JsonProperty("offers")]
        public List<OfferJson> Offers { get; set; }

        [JsonProperty("offers.cursor")]
        public List<OfferCursorJson> OfferCursors { get; set; }
    }
}
