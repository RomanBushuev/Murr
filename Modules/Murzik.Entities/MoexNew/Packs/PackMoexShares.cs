using Murzik.Entities.MoexNew.Share;
using System;
using System.Collections.Generic;

namespace Murzik.Entities.MoexNew.Packs
{
    public class PackMoexShares
    {
        public IReadOnlyCollection<ShareDescription> Shares { get; set; }

        public DateTime ValidDate { get; set; }
    }
}
