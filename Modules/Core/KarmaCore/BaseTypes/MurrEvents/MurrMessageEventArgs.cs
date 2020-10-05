using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes.MurrEvents
{
    public class MurrMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MurrMessageType MurrMessageType { get; set; }
    }
}
