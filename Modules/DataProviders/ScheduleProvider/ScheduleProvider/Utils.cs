using System;
using System.Collections.Generic;
using System.Text;
using NCrontab;
using System.Json;
using Newtonsoft.Json;

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
            if (dateTime.HasValue)
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
                var schedule = CrontabSchedule.Parse(template);
                var nextDate = schedule.GetNextOccurrence(currentDate);
                return nextDate;
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

        public static Dictionary<string, object> ConvertJsonToParams(string json)
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return dictionary;
        }


        public static DateTime ChangeParmas(string value)
        {
            if (value == "now")
                return DateTime.Now;
            if (value == "today")
                return DateTime.Today;
            if (value == "yesterday")
                return DateTime.Today.AddDays(-1);
            if (value == "tomorrow")
                return DateTime.Today.AddDays(1);
            throw new Exception("Error in ChangeParmas");
        }

        public static bool IsUseTemplate(string postgreSqlType, Type CSharpType)
        {
            return IsUseTemplate(postgreSqlType, CSharpType.ToString());
        }

        public static bool IsUseTemplate(string postgreSqlType, string CSharpType)
        {
            if (postgreSqlType == "timestamp without time zone" && CSharpType == "System.String")
                return true;

            return false;
        }

        public static Dictionary<string, object> ConvertValues(Dictionary<string,object> jsonValues, 
            Dictionary<string, string> procedureTypes)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            return result;
        }
    }
}
