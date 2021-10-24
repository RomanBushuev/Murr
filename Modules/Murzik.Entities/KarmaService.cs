using Murzik.Entities.Enumerations;

namespace Murzik.Entities
{
    public class KarmaService
    {
        public long ServiceId { get; set; }

        public string ServiceTitle { get; set; }

        public ServiceStatuses ServiceStatus { get; set; }
    }
}