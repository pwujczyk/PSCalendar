using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSCalendarTools;

namespace PSCalendar.Commands
{
    class EndDateTools
    {
        public static DateTime GetEndDate(DateTime startDate, string end, string duration)
        {
            //if (startDate.TimeOfDay.TotalSeconds==0)
            //{
            //    return startDate;
            //}
            DateTime endDate = startDate.AddHours(1);
            if (end.NotEmpty())
            {
                endDate = DateTime.Parse(end);
            }
            else if (duration.NotEmpty())
            {
                endDate = startDate.Add(ParseDuration(duration));
            }
            return endDate;
        }

        public static TimeSpan ParseDuration(string duration)
        {
            
            if (duration.EndsWith("m"))
            {
                return TimeSpan.FromMinutes(int.Parse(duration.Trim('m')));
            }
            if (duration.EndsWith("h"))
            {
                return TimeSpan.FromHours(float.Parse(duration.Trim('h')));
            }
            if (duration.EndsWith("d"))
            {
                return TimeSpan.FromDays(float.Parse(duration.Trim('d')));
            }
            throw new Exception("Duration letter not known");
        }
    }
}
