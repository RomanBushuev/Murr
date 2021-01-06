using KarmaCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Enumerations
{
    public enum SaverTypes
    {
        [MurrDbAttribute("UNDEFINED")]
        Undefined = 0,

        [MurrDbAttribute("XML")]
        Xml = 1,
    }
}
