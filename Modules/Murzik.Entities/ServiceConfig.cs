namespace Murzik.Entities
{
    public class ServiceConfig
    {
        public string ServiceName { get; set; }

        public long Interval { get; set; }

        public string Configuration { get; set; }

        public string KarmaAdmin { get; set; }

        public string KarmaDownloader { get; set; }
    }
}
