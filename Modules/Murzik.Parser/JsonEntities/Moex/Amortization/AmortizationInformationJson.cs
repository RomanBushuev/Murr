using Newtonsoft.Json;
using System.Collections.Generic;

namespace Murzik.Parser.JsonEntities.Moex
{
    internal class AmortizationInformationJson
    {
        [JsonProperty("charsetinfo")]
        public Charsetinfo Charsetinfo { get; set; }

        [JsonProperty("amortizations")]
        public List<AmortizationJson> Amortizations { get; set; }

        [JsonProperty("amortizations.cursor")]
        public List<AmortizationCursorJson> AmortizationCursors { get; set; }
    }
}
