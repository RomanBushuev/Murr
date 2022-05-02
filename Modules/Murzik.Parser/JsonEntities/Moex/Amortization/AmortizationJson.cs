using Newtonsoft.Json;
using System;

namespace Murzik.Parser.JsonEntities.Moex
{
    internal class AmortizationJson
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
        [JsonProperty("amortdate")]
        public DateTime? AmortizationDate { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        [JsonProperty("facevalue")]
        public decimal? FaceValue { get; set; }

        /// <summary>
        /// Непогашенный номинал
        /// </summary>
        [JsonProperty("initialfacevalue")]
        public decimal? InitialFaceValue { get; set; }

        /// <summary>
        /// Валюта номинала
        /// </summary>
        [JsonProperty("faceunit")]
        public string FaceUnit { get; set; }

        /// <summary>
        /// Выплата (использовать ее)
        /// </summary>
        [JsonProperty("value")]
        public decimal? Value { get; set; }

        /// <summary>
        /// Процент выплат
        /// </summary>
        [JsonProperty("valueprc")]
        public decimal? ValuePrc { get; set; }

        /// <summary>
        /// Размер выплаты в рублях
        /// </summary>
        [JsonProperty("value_rub")]
        public decimal? ValueRub { get; set; }

        /// <summary>
        /// Инструмент
        /// </summary>
        [JsonProperty("data_source")]
        public string DataSource { get; set; }

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
