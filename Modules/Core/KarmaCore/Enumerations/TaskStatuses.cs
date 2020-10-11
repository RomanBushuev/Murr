using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    public enum TaskStatuses
    {
        [MurrDbAttribute("UNDEFINED")]
        Undefined = 0,

        [MurrDbAttribute("CREATING")]
        Creating = 1,

        [MurrDbAttribute("CREATED")]
        Created = 2,

        [MurrDbAttribute("RUNNING")]
        Running = 3,

        [MurrDbAttribute("DONE")]
        Done = 4,

        [MurrDbAttribute("ERROR")]
        Error = 5,
    }
}
