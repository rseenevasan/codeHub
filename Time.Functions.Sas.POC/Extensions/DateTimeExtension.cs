using System;
using System.Collections.Generic;

namespace Paycor.Time.Core
{
    public static class DateTimeExtension
    {
        public static IEnumerable<DateTime> Iterate(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        public static double TotalMinutes(this DateTime first, DateTime second)
        {
            var ts = (second - first);
            return ts.TotalMinutes;
        }
        public static DateTime GetEndOfDateTime(this DateTime current)
        {
            return new DateTime(current.Year, current.Month, current.Day, 23, 59, 59);
        }

        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime WeekStart(this DateTime dt, DayOfWeek start)
        {
            int diff = dt.DayOfWeek - start;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime WeekStartWithTime(this DateTime dt, DayOfWeek start)
        {
            int diff = dt.DayOfWeek - start;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff);
        }

        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }
    }
}
