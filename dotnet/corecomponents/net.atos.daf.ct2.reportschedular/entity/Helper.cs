using System;
using net.atos.daf.ct2.reportscheduler.ENUM;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class Helper
    {
        public DateTime GetNextFrequencyTime(long currentdate, TimeFrequenyType timeFrequenyType)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var nextDate = date.AddDays(1);
            switch (timeFrequenyType)
            {

                case TimeFrequenyType.Daily:
                    nextDate = date.AddDays(1);
                    break;
                case TimeFrequenyType.Weekly:
                    nextDate = date.AddDays(7);
                    break;
                case TimeFrequenyType.BiWeekly:
                    nextDate = date.AddDays(14);
                    break;
            }
            return nextDate;
        }

        public Tuple<DateTime, DateTime> GetNextQuarterTime(long currentdate)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var quarterNumber = ((date.Month - 1) / 3) + 1;
            var firstDayOfQuarter = new DateTime(date.Year, ((quarterNumber - 1) * 3) + 1, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            var nextQuarter = quarterNumber + 1;
            var firstDayOfnextQuarter = new DateTime(date.Year, ((nextQuarter - 1) * 3) + 1, 1);
            var lastDayOfnextQuarter = firstDayOfnextQuarter.AddMonths(3).AddDays(-1);
            return Tuple.Create(firstDayOfnextQuarter, lastDayOfnextQuarter);
        }


        public Tuple<DateTime, DateTime> GetNextMonthlyTime(long currentdate)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var nextMonth = date.AddMonths(1);
            var startDate = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return Tuple.Create(startDate, endDate);
        }


    }
}
