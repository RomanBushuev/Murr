using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Entities
{
    public class KarmaDownloadJob
    {
        public long TaskId { get; set; }
        public long TaskTemplateId { get; set; }
        public TaskStatuses TaskStatuses { get; set; }
        public long SaverTemplateId { get; set; }
    }
}
