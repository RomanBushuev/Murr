using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface ICsvReaderAgent
    {
        Task<IReadOnlyCollection<BondDescription>> ReadBondDescriptionAsync(string filename);

        Task<IReadOnlyCollection<BondDataRow>> ReadBondDataRawAsync(string filename);

        Task<IReadOnlyCollection<ShareDescription>> ReadShareDescriptions(string filename);
    }
}
