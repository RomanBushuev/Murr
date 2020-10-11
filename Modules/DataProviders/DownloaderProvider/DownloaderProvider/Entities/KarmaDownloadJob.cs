using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.Entities
{
    public class KarmaDownloadJob
    {
        public long TaskId { get; set; }
        public long TaskTemplateId { get; set; }
        public TaskStatuses TaskStatuses { get; set; }
    }
}
