namespace Murzik.Entities
{
    public class SchedulerServiceConfige
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

        /// <summary>
        /// Карма агент
        /// </summary>
        public string KarmaAdmin { get; set; }

        /// <summary>
        /// Карма загрузчик
        /// </summary>
        public string KarmaDownloader { get; set; }
    }
}
