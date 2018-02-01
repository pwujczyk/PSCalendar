using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCalendar
{
    public static class Extensions
    {
        public static DateTime GetFirstWeekDay(this DateTime that)
        {
            int delta = DayOfWeek.Monday - that.DayOfWeek;
            DateTime monday = that.AddDays(delta);
            return monday;
        }

        public static DateTime GetLastWeekDay(this DateTime that)
        {
            int delta = DayOfWeek.Sunday - that.DayOfWeek;
            DateTime sunday= that.AddDays(delta);
            return sunday;
        }

        public static DateTime GetFirstMonthDay(this DateTime that)
        {
            var month = new DateTime(that.Year, that.Month, 1);
            return month;
        }

        public static DateTime GetLastMonthDay(this DateTime that)
        {
            var month = new DateTime(that.Year, that.Month, 1);
            DateTime last = month.AddMonths(1).AddDays(-1);
            return last;
        }

        public static class CommonExtensions
        {
            public static T ParseEnum<T>(string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
        }
    }
}
