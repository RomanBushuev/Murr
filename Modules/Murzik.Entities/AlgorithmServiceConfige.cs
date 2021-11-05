namespace Murzik.Entities
{
    public class AlgorithmServiceConfige
    {
        /// <summary>
        /// Наименование сервиса
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Интервал вызова в секундах
        /// </summary>
        public long Interval { get; set; }

        /// <summary>
        /// Production или Development
        /// </summary>
        public string Configuration { get; set; }
    }
}
