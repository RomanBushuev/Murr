using Newtonsoft.Json;
using System;

namespace Murzik.Parser.JsonEntities.Moex
{
    public class CouponJson
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
        /// Дата купона
        /// </summary>
        [JsonProperty("coupondate")] 
        public DateTime? CouponDate { get; set; }

        [JsonProperty("recorddate")] 
        public DateTime? RecordDate { get; set; }

        [JsonProperty("startdate")] 
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Номинал
        /// </summary>
        [JsonProperty("initialfacevalue")] 
        public decimal? InitialFacevalue { get; set; }

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
        /// Выплата
        /// </summary>
        [JsonProperty("value")] 
        public decimal? Value { get; set; }

        /// <summary>
        /// Ставка купона
        /// </summary>
        [JsonProperty("valueprc")] 
        public decimal? ValuePrc { get; set; }

        /// <summary>
        /// Размер выплаты в рублях
        /// </summary>
        [JsonProperty("value_rub")] 
        public decimal? ValueRub { get; set; }
    }
}