using Murzik.Entities.Moex;
using System.Collections.Generic;

namespace Murzik.Interfaces
{
    public interface ICsvSaver
    {
        public void Save(IReadOnlyCollection<MoexShareDataRow> moexShares, string connection);

        public void Save(IReadOnlyCollection<MoexBondDataRow> moexBonds, string connection);
    }
}
