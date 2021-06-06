using KarmaCore.BaseTypes;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoexDataProvider
{
    public class MoexSaverProvider : IMoexSaver
    {
        public IReadOnlyCollection<MoexEntity> moexEntities { get; set; }
        public bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
