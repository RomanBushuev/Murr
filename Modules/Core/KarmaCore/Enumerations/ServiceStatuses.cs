using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    public enum ServiceStatuses
    {
        [MurrDbAttribute("UNDEFINED")]
        Undefined = 0,

        [MurrDbAttribute("RUNNING")]
        Running = 1,

        [MurrDbAttribute("STOPPING")]
        Stopping = 2,
    }
}
