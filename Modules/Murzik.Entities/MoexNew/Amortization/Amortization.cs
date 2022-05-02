using System;

namespace Murzik.Entities.MoexNew.Amortization
{
    public class Amortization
    {
        /// <summary>
        /// Isin
        /// </summary>
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
        /// Дата оферты
        /// </summary>
        public DateTime? AmortizationDate { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        public decimal? FaceValue { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        public decimal? InitialFaceValue { get; set; }

        /// <summary>
        /// Валюта номинала
        /// </summary>
        public string FaceUnit { get; set; }

        /// <summary>
        /// Выплата (использовать ее)
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Процент выплат
        /// </summary>
        public decimal? ValuePrc { get; set; }

        /// <summary>
        /// Размер выплаты в рублях
        /// </summary>
        public decimal? ValueRub { get; set; }

        /// <summary>
        /// Инструмент
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Инструмент
        /// </summary>
        public string Secid { get; set; }

        /// <summary>
        /// Торговая доска
        /// </summary>
        public string PrimaryBoardId { get; set; }
    }
}
