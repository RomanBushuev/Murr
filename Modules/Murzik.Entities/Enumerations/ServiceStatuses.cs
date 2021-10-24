using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.Entities.Enumerations
{
    public enum ServiceStatuses
    {
        [MurrDb("UNDEFINED")]
        Undefined = 0,

        [MurrDb("RUNNING")]
        Running = 1,

        [MurrDb("STOPPING")]
        Stopping = 2,

        [MurrDb("FINISHED")]
        Finisehd = 3
    }
}
