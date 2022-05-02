using Murzik.Entities.MoexNew.Amortization;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Offer;

namespace Murzik.Interfaces
{
    public interface IJsonMoexParser
    {
        /// <summary>
        /// Конвертирование данных из данных Moex
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        CouponInformation ConvertToCouponInformationAndGetLast(string json);

        /// <summary>
        /// Конвертируем из Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        CouponInformation ConvertToCouponInformation(string json);

        /// <summary>
        /// Конвертирование данных из данных Moex
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        AmortizationInformation ConvertToAmortizationInformationAndGetLast(string json);

        /// <summary>
        /// Конвертирование из Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        AmortizationInformation ConvertToAmortizationInformation(string json);

        /// <summary>
        /// Конвертирование данных из данных Moex
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        OfferInformation ConvertToOfferInformationAndGetLast(string json);

        /// <summary>
        /// Конвертируем в нормальный json
        /// </summary>
        /// <param name="couponInformation"></param>
        /// <returns></returns>
        string ConvertCouponToJson(CouponInformation couponInformation);

    }
}
