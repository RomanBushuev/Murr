using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using System;

namespace KarmaCore.BaseTypes.Logger
{
    public class MurrLogger
    {
        public event EventHandler<MurrMessageEventArgs> ProcessCompleted;

        public void Notify(string message, MurrMessageType murrMessageType = MurrMessageType.Information)
        {
            ProcessCompleted?.Invoke(this, new MurrMessageEventArgs()
            {
                Message = message,
                MurrMessageType = murrMessageType
            });
        }
    }
}
