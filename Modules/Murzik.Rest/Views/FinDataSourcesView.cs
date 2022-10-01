using Newtonsoft.Json;

namespace Murzik.Rest.Views
{
    public class FinDataSourcesView
    {
        [JsonProperty("finDataSoruceId")]
        public long FinDataSoruceId { get; set; }

        [JsonProperty("finDataSourceIdent")]
        public string FinDataSourceIdent { get; set; }
    }
}
