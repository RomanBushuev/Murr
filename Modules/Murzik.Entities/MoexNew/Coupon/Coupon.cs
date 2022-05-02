using System;

namespace Murzik.Entities.MoexNew.Coupon
{
    /// <summary>
    /// Купон
    /// </summary>
    public class Coupon
    {
        public string Isin { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Объем эмиссии в млн
        /// </summary>
        public decimal? IssueValue { get; set; }

        /// <summary>
        /// Дата купона
        /// </summary>
        public DateTime? CouponDate { get; set; }

        /// <summary>
        /// Дата записи
        /// </summary>
        public DateTime? RecordDate { get; set; }

        /// <summary>
        /// Дата начала купона (не использовать)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Номинал
        /// </summary>
        public decimal? InitialFacevalue { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Валюта номинала
        /// </summary>
        public string FaceUnit { get; set; }

        /// <summary>
        /// Выплата
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Ставка купона
        /// </summary>
        public decimal? ValuePrc { get; set; }

        /// <summary>
        /// Размер выплаты в рублях
        /// </summary>
        public decimal? ValueRub { get; set; }
    }
}
