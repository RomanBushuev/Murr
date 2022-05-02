using System;

namespace Murzik.Entities.MoexNew.Offer
{
    public class Offer
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
        /// Дата оферты
        /// </summary>
        public DateTime? OfferDate { get; set; }

        /// <summary>
        /// Дата начала принятия решения по оферте
        /// </summary>
        public DateTime? OfferDateStart { get; set; }

        /// <summary>
        /// Дата окончания принятия решения по оферте
        /// </summary>
        public DateTime? OfferDateEnd { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Валюта номинала
        /// </summary>
        public string FaceUnit { get; set; }

        /// <summary>
        /// Стоимость (100 - это 100%), сколько будет выплачено денег
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Выплата (но не использовать)
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Выплата
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        /// Тип офферты
        /// </summary>
        public string OfferType { get; set; }

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
