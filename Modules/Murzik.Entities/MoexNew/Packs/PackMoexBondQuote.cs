using Murzik.Entities.MoexNew.Bond;
using System.Collections.Generic;

namespace Murzik.Entities.MoexNew.Packs
{
    public class PackMoexBondQuote
    {
        public IReadOnlyCollection<BondDataRow> Bonds { get; set; }
        public List<string> QuoteSources { get; set; }
    }
}
