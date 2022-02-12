using Murzik.Entities.MoexNew.Coupon;

namespace Murzik.Interfaces
{
    public interface IJsonMoexParser
    {
        /// <summary>
        /// Конверирование данных из данных Moex
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        CouponInformation ConvertFromRootJson(string json);

        /// <summary>
        /// Конвертируем в нормальный json
        /// </summary>
        /// <param name="couponInformation"></param>
        /// <returns></returns>
        string ConvertToJson(CouponInformation couponInformation);

        /// <summary>
        /// Конвертируем из Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        CouponInformation ConvertToCouponInformation(string json);
    }
}
