using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Exceptions
{
    public class KarmaCoreException :Exception
    {
        private readonly string _message;
        
        public KarmaCoreException(string message)
            : base(message)
        {
            _message = message;
        }

        public string KarmaMessage { get { return _message; } }
    }
}
