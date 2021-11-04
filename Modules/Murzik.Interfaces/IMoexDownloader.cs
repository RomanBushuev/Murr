using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IMoexDownloader
    {
        public Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRow(DateTime date);

        public Task<IReadOnlyCollection<ShareDataRow>> DownloadShareDataRow(DateTime date);
    }
}
