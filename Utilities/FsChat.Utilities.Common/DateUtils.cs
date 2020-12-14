using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Utilities.Common
{
    public static class DateUtils
    {
        public static IEnumerable<Tuple<int, string, int>> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            while (iterator <= limit)
            {
                yield return Tuple.Create(
                    iterator.Month,
                    iterator.ToString("MMM"),
                    iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }

        public static IEnumerable<Tuple<int, int, string, int, DayOfWeek>> WeeksBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, startDate.Day);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, endDate.Day);
                limit = startDate;
            }

            while (iterator <= limit)
            {
                yield return Tuple.Create(
                    iterator.Day,
                    iterator.Month,
                    iterator.ToString("MMM"),
                    iterator.Year,
                    iterator.DayOfWeek);
                iterator = iterator.AddDays(8 - (int)iterator.DayOfWeek);
            }
        }
    }
}
