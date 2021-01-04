using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Entities
{
    public class KarmaService
    {
        public long ServiceId { get; set; }

        public string ServiceTitle { get; set; }

        public ServiceStatuses ServiceStatus { get; set; }
    }
}
