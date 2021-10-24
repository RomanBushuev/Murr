using Murzik.Entities.Enumerations;

namespace Murzik.Entities
{
    public class KarmaDownloadJob
    {
        public long TaskId { get; set; }
        public long TaskTemplateId { get; set; }
        public TaskStatuses TaskStatuses { get; set; }
        public long? SaverTemplateId { get; set; }
    }
}
