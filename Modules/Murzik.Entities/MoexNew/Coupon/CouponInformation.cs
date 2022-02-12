using System.Collections.Generic;

namespace Murzik.Entities.MoexNew.Coupon
{
    public class CouponInformation
    {
        public CouponCursor[] CouponCursors { get; set; } = new CouponCursor[] { };

        public Coupon[] Coupons { get; set; } = new Coupon[] { };
    }
}
