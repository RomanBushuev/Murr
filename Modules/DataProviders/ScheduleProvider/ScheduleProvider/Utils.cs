using System;
using System.Collections.Generic;
using System.Text;
using NCrontab;

namespace ScheduleProvider
{
    public static class Utils
    {
        /// <summary>
        /// Получить новую дату
        /// </summary>
        /// <param name="dateTime">Предполагаемое следующее время</param>
        /// <param name="currentDate">Текущее время</param>
        /// <param name="template">Шаблон</param>
        /// <returns></returns>
        public static DateTime GetNextDateTime(DateTime? dateTime,
            DateTime currentDate,
            string template)
        {
            if(dateTime.HasValue)
            {
                if (MakeNextDate(dateTime, currentDate))
                {
                    var schedule = CrontabSchedule.Parse(template);
                    var nextDate = schedule.GetNextOccurrence(currentDate);
                    return nextDate;
                }
                else
                {
                    return dateTime.Value;
                }
            }
            else
            {
                return currentDate;
            }
        }

        /// <summary>
        /// Необходимо сформировать следующее время
        /// </summary>
        /// <param name="nextDate">Следующее время</param>
        /// <param name="currentDate">Текущее время</param>
        /// <returns></returns>
        public static bool MakeNextDate(DateTime? nextDate, 
            DateTime currentDate)
        {
            if (!nextDate.HasValue)
                return true;

            return nextDate.Value <= currentDate;
        }
    }
}
