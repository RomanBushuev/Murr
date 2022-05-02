using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Share;
using Murzik.Entities.MoexNew.Offer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    /// <summary>
    /// Загрузка с Moex
    /// </summary>
    public interface IMoexDownloader
    {
        /// <summary>
        /// Загрузка информации за день
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<BondDataRow>> DownloadBondDataRowAsync(DateTime date);

        /// <summary>
        /// Загрузка информации за день
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<ShareDataRow>> DownloadShareDataRowAsync(DateTime date);

        /// <summary>
        /// Загрузка купонов
        /// </summary>
        /// <param name="amountOfPageToDownload"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Coupon>> DownloadCouponsAsync(long? amountOfPageToDownload = null);

        /// <summary>
        /// Загрузка погашений
        /// </summary>
        /// <param name="amountOfPageToDownload"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Amortization>> DownloadAmortizationsAsync(long? amountOfPageToDownload = null);

        /// <summary>
        /// Загрузка офферт
        /// </summary>
        /// <param name="amountOfPageToDownload"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Offer>> DownloadOffersAsync(long? amountOfPageToDownload = null);
    }
}
