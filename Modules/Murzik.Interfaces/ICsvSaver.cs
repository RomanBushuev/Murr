using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Offer;
using Murzik.Entities.MoexNew.Share;
using System.Collections.Generic;

namespace Murzik.Interfaces
{
    public interface ICsvSaver
    {
        void Save(IReadOnlyCollection<ShareDataRow> shares, string connection);

        void Save(IReadOnlyCollection<BondDataRow> bonds, string connection);

        string Save(IReadOnlyCollection<Coupon> coupons, long taskId);

        string Save(IReadOnlyCollection<Amortization> amortizations, long taskId);

        string Save(IReadOnlyCollection<Offer> offers, long taskId);
    }
}
