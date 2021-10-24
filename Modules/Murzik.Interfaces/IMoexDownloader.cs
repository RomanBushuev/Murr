using Murzik.Entities.Moex;
using Murzik.Entities.MoexNew;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface IMoexDownloader
    {
        public Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRow(DateTime date);

        public Task<IReadOnlyCollection<MoexShareDataRow>> DownloadShareDataRow(DateTime date);
    }
}
