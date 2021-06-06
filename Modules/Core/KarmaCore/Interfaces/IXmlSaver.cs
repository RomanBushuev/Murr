using KarmaCore.BaseTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Interfaces
{
    public interface ISaver
    {
        public bool Save();
    }

    public interface IXmlSaver : ISaver
    {
        
    }

    public interface IMoexSaver : ISaver
    {
    }
}
