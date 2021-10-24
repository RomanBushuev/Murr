using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Interfaces
{
    public interface ISchedulerActions
    {
        DateTime GetNextDateTime(DateTime? dateTime, DateTime currentDate, string template);

        bool MakeNextDate(DateTime? nextDate, DateTime currentDate);

        Dictionary<string, object> ConvertJsonToParams(string json);

        DateTime ChangeParmas(string value);

        bool IsUseTemplate(string postgreSqlType, Type CSharpType);

        bool IsUseTemplate(string postgreSqlType, string CSharpType);

        Task CheckJob();
    }
}
