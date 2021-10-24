using System;

namespace Murzik.DownloaderProvider.DbEntities
{
    public class DbProcedureTask
    {
        public long ProcedureTaskId { get; set; }

        public string ProcedureTitle { get; set; }

        public bool ProcedureIsUse { get; set; }

        public string ProcedureParams { get; set; }

        public string ProcedureTemplate { get; set; }

        public DateTime? ProcedureLastRun { get; set; }

        public DateTime? ProcedureNextRun { get; set; }
    }
}
