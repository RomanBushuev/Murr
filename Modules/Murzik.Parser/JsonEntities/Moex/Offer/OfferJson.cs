using Newtonsoft.Json;
using System;

namespace Murzik.Parser.JsonEntities.Moex.Offer
{
    public class OfferJson
    {
        [JsonProperty("isin")]
        public string Isin { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Объем эмиссии в млн
        /// </summary>
        [JsonProperty("issuevalue")]
        public decimal? IssueValue { get; set; }

        /// <summary>
        /// Дата оферты
        /// </summary>
        [JsonProperty("offerdate")]
        public DateTime? OfferDate { get; set; }

        /// <summary>
        /// Дата начала принятия решения по оферте
        /// </summary>
        [JsonProperty("offerdatestart")]
        public DateTime? OfferDateStart { get; set; }

        /// <summary>
        /// Дата окончания принятия решения по оферте
        /// </summary>
        [JsonProperty("offerdateend")]
        public DateTime? OfferDateEnd { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        [JsonProperty("facevalue")]
        public decimal FaceValue { get; set; }

        /// <summary>
        /// Валюта номинала
        /// </summary>
        [JsonProperty("faceunit")]
        public string FaceUnit { get; set; }

        /// <summary>
        /// Стоимость (100 - это 100%), сколько будет выплачено денег
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Выплата (но не использовать)
        /// </summary>
        [JsonProperty("value")]
        public decimal? Value { get; set; }

        /// <summary>
        /// Выплата
        /// </summary>
        [JsonProperty("agent")]
        public string Agent { get; set; }

        /// <summary>
        /// Тип офферты
        /// </summary>
        [JsonProperty("offertype")]
        public string OfferType { get; set; }

        /// <summary>
        /// Инструмент
        /// </summary>
        [JsonProperty("secid")]
        public string Secid { get; set; }

        /// <summary>
        /// Торговая доска
        /// </summary>
        [JsonProperty("primary_boardid")]
        public string PrimaryBoardId { get; set; }
    }
}
