using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Share;
using System.Collections.Generic;

namespace Murzik.Interfaces
{
    public interface ICsvSaver
    {
        void Save(IReadOnlyCollection<ShareDataRow> shares, string connection);

        void Save(IReadOnlyCollection<BondDataRow> bonds, string connection);

        string Save(IReadOnlyCollection<Coupon> coupons, long taskId);
    }
}
