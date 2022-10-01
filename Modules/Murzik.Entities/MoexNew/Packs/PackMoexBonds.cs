using Murzik.Entities.MoexNew.Bond;
using System;
using System.Collections.Generic;

namespace Murzik.Entities.MoexNew.Packs
{
    public  class PackMoexBonds
    {
        public IReadOnlyCollection<BondDescription> Bonds { get; set; }

        public DateTime ValidDate { get; set; }
    }
}
